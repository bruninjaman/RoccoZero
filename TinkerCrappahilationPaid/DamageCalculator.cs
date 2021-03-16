using System;
using System.Collections.Generic;
using System.Linq;

using Divine;
using Divine.SDK.Extensions;

using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using TinkerCrappahilationPaid.Abilities;

namespace TinkerCrappahilationPaid
{
    public class DamageCalculator
    {
        private readonly TinkerCrappahilationPaid _main;
        public List<ActiveAbility> AllAbilities = new List<ActiveAbility>();
        public Dictionary<Hero, TargetClass> DamageDict;
        private AbilitiesInCombo Abilities => _main.AbilitiesInCombo;
        private Config Config => _main.Config;
        private Hero Me => _main.Me;
        public DamageCalculator(TinkerCrappahilationPaid main)
        {
            _main = main;
            DamageDict = new Dictionary<Hero, TargetClass>();
            UpdateManager.CreateIngameUpdate(150, Updater);
            UpdateManager.CreateIngameUpdate(500, PrepareItemsForCombo);
        }

        public void Updater()
        {
            var targets = EntityManager.GetEntities<Hero>().Where(x => x.IsValid &&
                                                                  x.IsAlive && !x.IsIllusion && x.IsVisible &&
                                                                  x.IsEnemy(Me) && !x.IsMagicImmune() &&
                                                                  (x.IsInRange(Me, 2500) ||
                                                                   !RendererManager.WorldToScreen(x.Position).IsZero));
            DamageDict = new Dictionary<Hero, TargetClass>();
            

            foreach (var target in targets)
            {
                
                if (!DamageDict.ContainsKey(target))
                {
                    DamageDict.Add(target, new TargetClass(target));
                }
                var mana = Me.Mana;
                var rearmCount = 0;
                var rearmed = true;
                var buffedUnderVeil = target.HasModifier("modifier_item_veil_of_discord_debuff");
                //var buffedUnderEthereal = target.HasModifier("modifier_item_ethereal_blade_ethereal");
                var debug = 0;
//                TinkerCrappahilationPaid.Log.Debug($"checking for {target.HeroId}");
                while (rearmed && mana > 0 && debug++ <= 100)
                {
                    try
                    {
                        rearmed = false;
                        foreach (var activeAbility in AllAbilities)
                        {
                            TargetClass gg;
                            DamageDict.TryGetValue(target, out gg);
                            
                            if (DamageDict.TryGetValue(target, out var t) && t.Health < 0)
                            {
                                if (rearmCount == 0)
                                {
                                    t.WillDieAfterFirstCast = t.Health <= 0;
//                                    TinkerCrappahilationPaid.Log.Warn($"Will die: {DamageDict[target].Health} Dmg: {DamageDict[target].DamageTakenFromFirstCast}");
                                }
                                continue;
                            }
                            if (rearmCount == 0 && !activeAbility.CanBeCasted)
                                continue;
                            switch (activeAbility)
                            {
                                case item_blink _:
                                    break;
                                case item_ghost _:
                                    break;
                                case item_soul_ring itemSoulRing:
                                    var extraMana = itemSoulRing.TotalManaRestore;
                                    mana += extraMana;
                                    //TinkerCrappahilationPaid.Log.Warn($"[Ability]: {itemSoulRing} ExtraMana: {extraMana}");
                                    break;
                                case item_sheepstick hex:
                                    if (mana < activeAbility.ManaCost || !hex.CanHit(target))
                                        continue;
                                    mana -= activeAbility.ManaCost;
                                    break;
                                case Rearm _:
                                    if (rearmCount == 0)
                                    {
                                        DamageDict[target].WillDieAfterFirstCast = DamageDict[target].Health <= 0;
                                        if (DamageDict[target].Health < 0)
                                        {
                                            DamageDict[target].WillDieAfterFirstCast = DamageDict[target].Health <= 0;
                                        }
                                    }
                                    if (mana < activeAbility.ManaCost)
                                        continue;
                                    mana -= activeAbility.ManaCost;
                                    rearmed = true;
                                    rearmCount++;
                                    //TinkerCrappahilationPaid.Log.Warn($"[Ability]: Rearm Count: {rearmCount}");
                                    break;
                                case item_veil_of_discord veil:
                                    if (mana < veil.ManaCost || buffedUnderVeil)
                                        continue;
                                    mana -= veil.ManaCost;
                                    buffedUnderVeil = true;
                                    //TinkerCrappahilationPaid.Log.Warn($"[Ability]: {veil} for debuff");
                                    break;
                                default:
                                    if (mana < activeAbility.ManaCost)
                                        continue;

                                    /*if (activeAbility.Ability.Id == AbilityId.item_ethereal_blade)
                                    {
                                        buffedUnderEthereal = true;
                                    }*/

                                    var extraMod = 0f;

                                    if (Abilities.Veil != null &&
                                        !target.HasModifier("modifier_item_veil_of_discord_debuff"))
                                        extraMod += Abilities.Veil.DamageAmplification;

                                    if (Abilities.EtherealBlade != null &&
                                        !target.HasModifier("modifier_item_ethereal_blade_ethereal"))
                                        extraMod += Abilities.EtherealBlade.DamageAmplification;
                                    
                                    var damage = activeAbility.DamageType == DamageType.Pure
                                        ? activeAbility.GetDamage(target)
                                        : activeAbility.GetDamage(target, extraMod);
//                                    TinkerCrappahilationPaid.Log.Debug($"{activeAbility.Ability.Id} Damage: {damage} {activeAbility.DamageType} {activeAbility.CanHit(target)}");
                                    if (damage > 0)
                                    {
                                        if (activeAbility.CanHit(target))
                                        {
                                            DamageDict[target].DamageTaken += damage;
                                            DamageDict[target].Health -= damage;

                                            /*DamageDict[target].DamageTakentWithoutRange += damage;
                                            DamageDict[target].HealthWithoutRange -= damage;*/

                                            if (rearmCount == 0)
                                            {
                                                DamageDict[target].DamageTakenFromFirstCast += damage;
                                                DamageDict[target].HealthAfterFirstCast -= damage;

                                                /*DamageDict[target].DamageTakenFromFirstCastWithoutRange += damage;
                                                DamageDict[target].HealthAfterFirstCastWithoutRange -= damage;*/
                                            }
                                            mana -= activeAbility.ManaCost;
                                        }
                                        /*else
                                        {
                                            DamageDict[target].DamageTakentWithoutRange += damage;
                                            DamageDict[target].HealthWithoutRange -= damage;

                                            if (rearmCount == 0)
                                            {
                                                DamageDict[target].DamageTakenFromFirstCastWithoutRange += damage;
                                                DamageDict[target].HealthAfterFirstCastWithoutRange -= damage;
                                            }

                                            continue;
                                        }*/
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    break;
                            }
                            /*TinkerCrappahilationPaid.Log.Info(
                                $"[Ability]: {activeAbility} on {target.HeroId}. Mana(ManaCost) {mana}({activeAbility.ManaCost})");*/
                            if (rearmCount == 0)
                            {
                                if (activeAbility != Abilities.Rearm &&
                                    activeAbility.Ability.Id != AbilityId.item_blink)
                                {
                                    DamageDict[target].AbilitiesForKillSteal.Add(activeAbility);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }

                if (debug >= 100)
                {
                    TinkerCrappahilationPaid.Log.Error($"ERROR IN LOOP debug ({debug}) >= 100 -> ");
                }

                rearmCount = 0;
                rearmed = true;
                buffedUnderVeil = target.HasModifier("modifier_item_veil_of_discord_debuff");
                debug = 0;
                mana = Me.Mana;
                while (rearmed && mana > 0 && debug++ <= 100)
                {
                    try
                    {
                        rearmed = false;
                        foreach (var activeAbility in AllAbilities)
                        {
                            if (rearmCount == 0 && !activeAbility.CanBeCasted)
                                continue;
                            switch (activeAbility)
                            {
                                case item_blink _:
                                    break;
                                case item_ghost _:
                                    break;
                                case item_soul_ring itemSoulRing:
                                    var extraMana = itemSoulRing.TotalManaRestore;
                                    mana += extraMana;
                                    //TinkerCrappahilationPaid.Log.Warn($"[Ability]: {itemSoulRing} ExtraMana: {extraMana}");
                                    break;
                                case item_sheepstick hex:
                                    if (mana < activeAbility.ManaCost || !hex.CanHit(target))
                                        continue;
                                    mana -= activeAbility.ManaCost;
                                    break;
                                case Rearm _:
                                    /*if (rearmCount == 0)
                                    {
                                        DamageDict[target].WillDieAfterFirstCast = DamageDict[target].Health <= 0;
                                    }*/
                                    if (mana < activeAbility.ManaCost)
                                        continue;
                                    mana -= activeAbility.ManaCost;
                                    rearmed = true;
                                    rearmCount++;
                                    //TinkerCrappahilationPaid.Log.Warn($"[Ability]: Rearm Count: {rearmCount}");
                                    break;
                                case item_veil_of_discord veil:
                                    if (mana < veil.ManaCost || buffedUnderVeil)
                                        continue;
                                    mana -= veil.ManaCost;
                                    buffedUnderVeil = true;
                                    //TinkerCrappahilationPaid.Log.Warn($"[Ability]: {veil} for debuff");
                                    break;
                                default:
                                    if (mana < activeAbility.ManaCost)
                                        continue;

                                    var extraMod = 0f;

                                    if (Abilities.Veil != null &&
                                        !target.HasModifier("modifier_item_veil_of_discord_debuff"))
                                        extraMod += Abilities.Veil.DamageAmplification;

                                    if (Abilities.EtherealBlade != null &&
                                        !target.HasModifier("modifier_item_ethereal_blade_ethereal"))
                                        extraMod += Abilities.EtherealBlade.DamageAmplification;

                                    var damage = activeAbility.DamageType == DamageType.Pure
                                        ? activeAbility.GetDamage(target)
                                        : activeAbility.GetDamage(target, extraMod);

                                    if (damage > 0)
                                    {
                                        DamageDict[target].DamageTakentWithoutRange += damage;
                                        DamageDict[target].HealthWithoutRange -= damage;

                                        if (rearmCount == 0)
                                        {
                                            DamageDict[target].DamageTakenFromFirstCastWithoutRange += damage;
                                            DamageDict[target].HealthAfterFirstCastWithoutRange -= damage;
                                        }
                                        mana -= activeAbility.ManaCost;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }

                if (debug >= 100)
                {
                    TinkerCrappahilationPaid.Log.Error($"ERROR (2) IN LOOP debug ({debug}) >= 100 -> ");
                }

                //var dmg = 0f;
                try
                {
                    //dmg = DamageDict[target].DamageTaken;
                }
                catch (Exception)
                {
                    // ignored
                }
                //TinkerCrappahilationPaid.Log.Warn($"Damage to {target.HeroId} -> {dmg} RearmCount: {rearmCount}");
            }


            /*foreach (var f in damageDict)
            {
                TinkerCrappahilationPaid.Log.Warn($"Damage to {f.Key.HeroId} -> {f.Value}");
            }*/
        }


        public void PrepareItemsForCombo()
        {
            var items = GetItems();
            AllAbilities = new List<ActiveAbility>
            {
                Abilities.Laser,
                Abilities.Rocket,
                Abilities.Rearm
            }.Where(x => Config.ItemsInCombo.Value.IsEnabled(x.Ability.Id.ToString())).ToList();
            foreach (var item in items)
            {
                var ability = _main.Context.AbilityFactory.GetAbility(item);
                if (ability is ActiveAbility ab)
                    AllAbilities.Add(ab);
            }
            AllAbilities = AllAbilities.OrderBy(y => Config.Priority.Value.GetPriority(y.Ability.Id.ToString())).ToList();
            /*TinkerCrappahilationPaid.Log.Error($"-----------------");
            var N = 0;
            foreach (var ability in _allAbilities)
            {
                TinkerCrappahilationPaid.Log.Error($"[{++N}] {ability}");
            }*/
        }

        private IEnumerable<Ability> GetItems()
        {
            var items = _main.Context.Inventory.Items.Where(x =>
                Config.ItemsInCombo.Value.IsEnabled(x.Id.ToString())).Select(z => z.Item);
            return items;
        }
    }
}