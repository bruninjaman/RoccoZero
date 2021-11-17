using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.GameConsole;
using Divine.Helpers;
using Divine.Update;

using TinkerEW.ItemsAndAbilities.Abilities;
using TinkerEW.ItemsAndAbilities.Items;

namespace TinkerEW
{
    internal sealed class Combo
    {
        internal readonly Menu Menu;
        internal readonly Hero? LocalHero;
        private readonly TargetSelector TargetSelector;
        private readonly LinkenBreaker LinkenBreaker;
        internal readonly Sleeper ComboSleeper = new Sleeper();
        internal readonly Sleeper BackSwingSleeper = new Sleeper();
        private bool FirstBlink;
        private bool DoubleShiva;
        internal Unit Target;
        internal Items Items = new Items();
        internal Abilities Abilities = new Abilities();
        private int LastAutoAttack;
        internal Dictionary<AbilityId, bool> UsedAbils;

        public Combo(Menu menu)
        {
            Menu = menu;
            LocalHero = EntityManager.LocalHero;
            TargetSelector = new TargetSelector(Menu);
            LinkenBreaker = new LinkenBreaker(this);
            UpdateManager.GameUpdate += UpdateManager_GameUpdate;
            LastAutoAttack = GameConsoleManager.GetInt32("dota_player_units_auto_attack_mode");
            UsedAbils = new Dictionary<AbilityId, bool>();
            GameConsoleManager.ExecuteCommand($"dota_player_units_auto_attack_mode {0}");
        }

        private void UpdateManager_GameUpdate()
        {
            if (LocalHero == null)
                return;

            Items.Update();
            Abilities.Update();

            Target = TargetSelector.GetTarget();
            if (Target != null)
            {

                if (Menu.ComboAbilitiesToggler.GetValue(AbilityId.tinker_defense_matrix)
                    && !UsedAbils.ContainsKey(AbilityId.tinker_defense_matrix)
                    && !ComboSleeper.Sleeping
                    && !LocalHero.HasModifier("modifier_tinker_defense_matrix")
                    && Abilities.defenseMatrix.CanBeCasted())
                {
                    Abilities.defenseMatrix.Cast(LocalHero);
                    ComboSleeper.Sleep((Abilities.defenseMatrix.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.tinker_defense_matrix, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_blink)
                    && !UsedAbils.ContainsKey(AbilityId.item_blink)
                    && !ComboSleeper.Sleeping
                    && Items.blink.CanBeCasted())
                {

                    switch (Menu.ComboBlinkMode.Value)
                    {
                        case "1`st in radius then to cursor":
                            if (!FirstBlink)
                            {
                                Items.blink.Cast(Target.Position.Extend(LocalHero.Position, 500));
                                FirstBlink = true;
                                ComboSleeper.Sleep((Items.blink.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                                UsedAbils.Add(AbilityId.item_blink, true);
                                return;
                            }
                            else
                            {
                                Items.blink.Cast(GameManager.MousePosition);
                                ComboSleeper.Sleep((Items.blink.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                                UsedAbils.Add(AbilityId.item_blink, true);
                                return;
                            }
                        case "In radius":
                            if (!FirstBlink)
                            {
                                Items.blink.Cast(Target.Position.Extend(LocalHero.Position, 500));
                                FirstBlink = true;
                                ComboSleeper.Sleep((Items.blink.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                                UsedAbils.Add(AbilityId.item_blink, true);
                                return;
                            }
                            else
                            {
                                Items.blink.Cast(Target.Position.Extend(GameManager.MousePosition, 500));
                                ComboSleeper.Sleep((Items.blink.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                                UsedAbils.Add(AbilityId.item_blink, true);
                                return;
                            }
                        case "To cursor":
                            Items.blink.Cast(GameManager.MousePosition);
                            ComboSleeper.Sleep((Items.blink.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                            UsedAbils.Add(AbilityId.item_blink, true);
                            return;
                    }
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_soul_ring)
                    && !UsedAbils.ContainsKey(AbilityId.item_soul_ring)
                    && !ComboSleeper.Sleeping
                    && Items.soulRing.CanBeCasted())
                {
                    Items.soulRing.Cast();
                    ComboSleeper.Sleep((Items.soulRing.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_soul_ring, true);
                    return;
                }

                LinkenBreaker.Activate();

                if (Menu.ComboDoubleShiva.Value
                    && Menu.ComboItemsToggler.GetValue(AbilityId.item_shivas_guard)
                    && !UsedAbils.ContainsKey(AbilityId.item_shivas_guard)
                    && !ComboSleeper.Sleeping
                    && DoubleShiva
                    && Items.shivasGuard.CanBeCasted())
                {
                    DoubleShiva = !Items.shivasGuard.Cast();
                    ComboSleeper.Sleep((Items.shivasGuard.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_shivas_guard, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_sheepstick)
                    && !UsedAbils.ContainsKey(AbilityId.item_sheepstick)
                    && !ComboSleeper.Sleeping
                    && Items.scytheOfVyse.CanBeCasted())
                {
                    Items.scytheOfVyse.Cast(Target);
                    ComboSleeper.Sleep((Items.scytheOfVyse.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_sheepstick, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_veil_of_discord)
                    && !UsedAbils.ContainsKey(AbilityId.item_veil_of_discord)
                    && !ComboSleeper.Sleeping
                    && Items.veilOfDiscord.CanBeCasted())
                {
                    Items.veilOfDiscord.Cast(Target.Position);
                    ComboSleeper.Sleep((Items.veilOfDiscord.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_veil_of_discord, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_ethereal_blade)
                    && !UsedAbils.ContainsKey(AbilityId.item_ethereal_blade)
                    && !ComboSleeper.Sleeping
                    && Items.etherealBlade.CanBeCasted())
                {
                    Items.etherealBlade.Cast(Target);
                    ComboSleeper.Sleep((Items.etherealBlade.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_ethereal_blade, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_ghost)
                    && !UsedAbils.ContainsKey(AbilityId.item_ghost)
                    && !ComboSleeper.Sleeping
                    && Items.ghostScepter.CanBeCasted())
                {
                    Items.ghostScepter.Cast();
                    ComboSleeper.Sleep((Items.ghostScepter.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_ghost, true);
                    return;
                }

                if (!Menu.ComboDoubleShiva.Value
                    && Menu.ComboItemsToggler.GetValue(AbilityId.item_shivas_guard)
                    && !UsedAbils.ContainsKey(AbilityId.item_shivas_guard)
                    && !ComboSleeper.Sleeping
                    && Items.shivasGuard.CanBeCasted())
                {
                    Items.shivasGuard.Cast();
                    ComboSleeper.Sleep((Items.shivasGuard.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_shivas_guard, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_dagon)
                    && !UsedAbils.ContainsKey(AbilityId.item_dagon)
                    && !ComboSleeper.Sleeping
                    && Items.dagon.CanBeCasted())
                {
                    Items.dagon.Cast(Target);
                    ComboSleeper.Sleep((Items.dagon.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_dagon, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_orchid)
                    && !UsedAbils.ContainsKey(AbilityId.item_orchid)
                    && !ComboSleeper.Sleeping
                    && Items.orchid.CanBeCasted())
                {
                    Items.orchid.Cast(Target);
                    ComboSleeper.Sleep((Items.orchid.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_orchid, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_bloodthorn)
                    && !UsedAbils.ContainsKey(AbilityId.item_bloodthorn)
                    && !ComboSleeper.Sleeping
                    && Items.bloodthorn.CanBeCasted())
                {
                    Items.bloodthorn.Cast(Target);
                    ComboSleeper.Sleep((Items.bloodthorn.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_bloodthorn, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_lotus_orb)
                    && !UsedAbils.ContainsKey(AbilityId.item_lotus_orb)
                    && !ComboSleeper.Sleeping
                    && Items.lotusOrb.CanBeCasted())
                {
                    Items.lotusOrb.Cast(LocalHero);
                    ComboSleeper.Sleep((Items.lotusOrb.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_lotus_orb, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_rod_of_atos)
                    && !UsedAbils.ContainsKey(AbilityId.item_rod_of_atos)
                    && !ComboSleeper.Sleeping
                    && Items.rodOfAtos.CanBeCasted())
                {
                    Items.rodOfAtos.Cast(Target);
                    ComboSleeper.Sleep((Items.rodOfAtos.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_rod_of_atos, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_glimmer_cape)
                    && !UsedAbils.ContainsKey(AbilityId.item_glimmer_cape)
                    && !ComboSleeper.Sleeping
                    && Items.glimmerCape.CanBeCasted())
                {
                    Items.glimmerCape.Cast(Target);
                    ComboSleeper.Sleep((Items.glimmerCape.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_glimmer_cape, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_guardian_greaves)
                    && !UsedAbils.ContainsKey(AbilityId.item_guardian_greaves)
                    && !ComboSleeper.Sleeping
                    && Items.guardianGreaves.CanBeCasted())
                {
                    Items.guardianGreaves.Cast();
                    ComboSleeper.Sleep((Items.guardianGreaves.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_guardian_greaves, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_nullifier)
                    && !UsedAbils.ContainsKey(AbilityId.item_nullifier)
                    && !ComboSleeper.Sleeping
                    && Items.nullifier.CanBeCasted())
                {
                    Items.nullifier.Cast(Target);
                    ComboSleeper.Sleep((Items.nullifier.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_nullifier, true);
                    return;
                }

                if (Menu.ComboItemsToggler.GetValue(AbilityId.item_eternal_shroud)
                    && !UsedAbils.ContainsKey(AbilityId.item_eternal_shroud)
                    && !ComboSleeper.Sleeping
                    && Items.eternalShroud.CanBeCasted())
                {
                    Items.eternalShroud.Cast();
                    ComboSleeper.Sleep((Items.eternalShroud.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_eternal_shroud, true);
                    return;
                }

                if (Menu.ComboAbilitiesToggler.GetValue(AbilityId.tinker_heat_seeking_missile)
                    && !UsedAbils.ContainsKey(AbilityId.tinker_heat_seeking_missile)
                    && !ComboSleeper.Sleeping
                    && Abilities.heatSeekingMissile.CanBeCasted())
                {
                    Abilities.heatSeekingMissile.Cast();
                    ComboSleeper.Sleep((Abilities.heatSeekingMissile.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.tinker_heat_seeking_missile, true);
                    return;
                }

                if (Menu.ComboAbilitiesToggler.GetValue(AbilityId.tinker_laser)
                    && !UsedAbils.ContainsKey(AbilityId.tinker_laser)
                    && !ComboSleeper.Sleeping
                    && Abilities.laser.CanBeCasted())
                {
                    Abilities.laser.Cast(Target);
                    ComboSleeper.Sleep((Abilities.laser.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.tinker_laser, true);
                    return;
                }

                if (Menu.ComboAbilitiesToggler.GetValue(AbilityId.tinker_march_of_the_machines)
                    && !UsedAbils.ContainsKey(AbilityId.tinker_march_of_the_machines)
                    && !ComboSleeper.Sleeping
                    && Abilities.marchOfTheMachines.CanBeCasted())
                {
                    Abilities.marchOfTheMachines.Cast(LocalHero.Position.Extend(Target.Position, 100));
                    ComboSleeper.Sleep((Abilities.marchOfTheMachines.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.tinker_march_of_the_machines, true);
                    return;
                }

                if (Menu.ComboAbilitiesToggler.GetValue(AbilityId.tinker_rearm)
                    && !UsedAbils.ContainsKey(AbilityId.tinker_rearm)
                    && !ComboSleeper.Sleeping
                    && Abilities.rearm.CanBeCasted())
                {
                    Abilities.rearm.Cast();
                    ComboSleeper.Sleep((Abilities.rearm.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.tinker_rearm, true);
                    if (!Menu.ComboDoubleShiva.Value || (Menu.ComboDoubleShiva.Value && !Items.shivasGuard.CanBeCasted()))
                    {
                        UsedAbils.Clear();
                    }
                    return;
                }

                if (Menu.ComboDoubleShiva.Value
                    && Menu.ComboItemsToggler.GetValue(AbilityId.item_shivas_guard)
                    && !UsedAbils.ContainsKey(AbilityId.item_shivas_guard)
                    && !ComboSleeper.Sleeping
                    && !DoubleShiva
                    && Items.shivasGuard.CanBeCasted())
                {
                    DoubleShiva = Items.shivasGuard.Cast();
                    ComboSleeper.Sleep((Items.shivasGuard.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                    UsedAbils.Add(AbilityId.item_shivas_guard, true);
                    UsedAbils.Clear();
                    return;
                }
            }
            else
            {
                UsedAbils.Clear();
            }
        }

        internal void Dispose()
        {
            FirstBlink = false;
            DoubleShiva = false;
            GameConsoleManager.ExecuteCommand($"dota_player_units_auto_attack_mode {LastAutoAttack}");
            UsedAbils.Clear();
            UpdateManager.GameUpdate -= UpdateManager_GameUpdate;
        }
    }
}