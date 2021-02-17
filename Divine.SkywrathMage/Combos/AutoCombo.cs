using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.Unit;
using Divine.SkywrathMage.Menus;

using Ensage.SDK.Extensions;
using Ensage.SDK.Menu.ValueBinding;

namespace Divine.SkywrathMage.Combos
{
    internal sealed class AutoCombo : BaseTaskHandler
    {
        private readonly BaseComboMenu ComboMenu;

        private readonly BaseAeonDiskMenu AeonDiskMenu;

        private readonly AutoComboMenu AutoComboMenu;

        private readonly SmartConcussiveShotMenu SmartConcussiveShotMenu;

        private readonly BaseBladeMailMenu BladeMailMenu;

        private readonly Abilities Abilities;

        private readonly BaseLinkenBreaker LinkenBreaker;

        private readonly BaseKillSteal KillSteal;

        public AutoCombo(Common common)
        {
            ComboMenu = common.MenuConfig.ComboMenu;
            AeonDiskMenu = common.MenuConfig.ComboMenu.AeonDiskMenu;
            AutoComboMenu = ((MoreMenu)common.MenuConfig.MoreMenu).AutoComboMenu;
            SmartConcussiveShotMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartConcussiveShotMenu;
            BladeMailMenu = common.MenuConfig.BladeMailMenu;

            Abilities = (Abilities)common.Abilities;
            LinkenBreaker = common.LinkenBreaker;
            KillSteal = common.KillSteal;

            if (AutoComboMenu.EnableItem)
            {
                RunAsync();
            }

            AutoComboMenu.EnableItem.Changed += EnableChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            AutoComboMenu.EnableItem.Changed -= EnableChanged;
        }

        private void EnableChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                RunAsync();
            }
            else
            {
                Cancel();
            }
        }

        protected override CUnit CurrentTarget
        {
            get
            {
                return UnitManager<CHero, Enemy, NoIllusion>.Units.Where(x => x.IsVisible && x.IsAlive && x.Distance2D(Owner) <= 1850 && Utils.AutoCombo(x))
                                                                  .OrderBy(x => x.Distance2D(Owner))
                                                                  .FirstOrDefault();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (AutoComboMenu.DisableWhenComboItem && ComboMenu.ComboHotkeyItem)
                {
                    return;
                }

                if (KillSteal.IsKillSteal || IsStopped || Owner.IsInvisible())
                {
                    return;
                }

                if (AutoComboMenu.OwnerMinHealthItem.Value > ((float)Owner.Health / Owner.MaximumHealth) * 100)
                {
                    return;
                }

                var target = CurrentTarget;
                if (IsNullTarget(target))
                {
                    return;
                }

                if (!AeonDiskMenu.EnableItem || target.ComboBreaker())
                {
                    return;
                }

                var targetModifiers = new TargetModifiers(target);
                if (BladeMailMenu.BladeMailItem && targetModifiers.ModifierReflect != null)
                {
                    return;
                }

                if (target.IsBlockMagicDamage(targetModifiers))
                {
                    return;
                }

                if (target.IsShieldAbilities(targetModifiers) && !targetModifiers.IsSilverDebuff)
                {
                    LinkenBreaker.RunAsync();
                    return;
                }

                // Hex
                var modifierStun = targetModifiers.ModifierStun;
                var modifierHex = targetModifiers.ModifierHex;
                var hex = Abilities.Hex;
                if (hex != null
                    && AutoComboMenu.ItemsSelection.PictureStates[hex.Name]
                    && hex.CanBeCasted
                    && hex.CanHit(target)
                    && modifierStun.IsModifier(0.3f)
                    && modifierHex.IsModifier(0.3f))
                {
                    hex.Cast(target);
                    await Task.Delay(hex.GetCastDelay(target), token);
                }

                // Orchid
                var orchid = Abilities.Orchid;
                if (orchid != null
                    && AutoComboMenu.ItemsSelection.PictureStates[orchid.Name]
                    && orchid.CanBeCasted
                    && orchid.CanHit(target))
                {
                    orchid.Cast(target);
                    await Task.Delay(orchid.GetCastDelay(target), token);
                }

                // Bloodthorn
                var bloodthorn = Abilities.Bloodthorn;
                if (bloodthorn != null
                    && AutoComboMenu.ItemsSelection.PictureStates[bloodthorn.Name]
                    && bloodthorn.CanBeCasted
                    && bloodthorn.CanHit(target))
                {
                    bloodthorn.Cast(target);
                    await Task.Delay(bloodthorn.GetCastDelay(target), token);
                }

                // MysticFlare
                var mysticFlare = Abilities.MysticFlare;
                if (AutoComboMenu.SpellsSelection.PictureStates[mysticFlare.Name]
                    && AutoComboMenu.MinHealthToUltItem.Value <= ((float)target.Health / target.MaximumHealth) * 100
                    && mysticFlare.CanBeCasted
                    && mysticFlare.CanHit(target))
                {
                    var position = mysticFlare.MysticFlarePrediction(Prediction, target);
                    mysticFlare.Cast(position);
                    await Task.Delay(mysticFlare.GetCastDelay(position), token);
                }

                // Nullifier
                var nullifier = Abilities.Nullifier;
                if (nullifier != null
                    && AutoComboMenu.ItemsSelection.PictureStates[nullifier.Name]
                    && nullifier.CanBeCasted
                    && nullifier.CanHit(target)
                    && modifierStun.IsModifier(0.5f)
                    && modifierHex.IsModifier(0.5f))
                {
                    nullifier.Cast(target);
                    await Task.Delay(nullifier.GetCastDelay(target), token);
                }

                // Atos
                var modifierAtos = targetModifiers.ModifierAtos;
                var atos = Abilities.Atos;
                if (atos != null
                    && AutoComboMenu.ItemsSelection.PictureStates[atos.Name]
                    && atos.CanBeCasted
                    && atos.CanHit(target)
                    && modifierAtos.IsModifier(0.5f)
                    && modifierStun.IsModifier(0.5f))
                {
                    atos.Cast(target);
                    await Task.Delay(atos.GetCastDelay(target), token);
                }

                // AncientSeal
                var ancientSeal = Abilities.AncientSeal;
                if (AutoComboMenu.SpellsSelection.PictureStates[ancientSeal.Name]
                    && ancientSeal.CanBeCasted
                    && ancientSeal.CanHit(target))
                {
                    ancientSeal.Cast(target);
                    await Task.Delay(ancientSeal.GetCastDelay(target), token);
                    return;
                }

                // Veil
                var veil = Abilities.Veil;
                if (veil != null
                    && AutoComboMenu.ItemsSelection.PictureStates[veil.Name]
                    && veil.CanBeCasted
                    && veil.CanHit(target))
                {
                    veil.Cast(target.Position);
                    await Task.Delay(veil.GetCastDelay(target.Position), token);
                }

                // Ethereal
                var ethereal = Abilities.Ethereal;
                if (ethereal != null
                    && AutoComboMenu.ItemsSelection.PictureStates[ethereal.Name]
                    && ethereal.CanBeCasted
                    && ethereal.CanHit(target))
                {
                    ethereal.Cast(target);
                    Helpers.MultiSleeper<string>.Sleep($"IsHitTime_{target.Name}_{ethereal.Name}", ethereal.GetHitTime(target));
                    await Task.Delay(ethereal.GetCastDelay(target), token);
                    return;
                }

                // Shivas
                var shivas = Abilities.Shivas;
                if (shivas != null
                    && AutoComboMenu.ItemsSelection.PictureStates[shivas.Name]
                    && shivas.CanBeCasted
                    && shivas.CanHit(target))
                {
                    shivas.Cast();
                    await Task.Delay(shivas.GetCastDelay(), token);
                }

                if (!Helpers.MultiSleeper<string>.Sleeping($"IsHitTime_{target.Name}_item_ethereal_blade") || targetModifiers.IsEthereal)
                {
                    // ConcussiveShot
                    var concussiveShot = Abilities.ConcussiveShot;
                    if (AutoComboMenu.SpellsSelection.PictureStates[concussiveShot.Name]
                        && concussiveShot.CanBeCasted
                        && concussiveShot.CanHit(target)
                        && Utils.ConcussiveShotTarget(SmartConcussiveShotMenu, target, concussiveShot.TargetHit))
                    {
                        concussiveShot.Cast();
                        await Task.Delay(concussiveShot.GetCastDelay(), token);
                    }

                    // ArcaneBolt
                    var arcaneBolt = Abilities.ArcaneBolt;
                    var arcaneBoltName = arcaneBolt.Name;
                    if (AutoComboMenu.SpellsSelection.PictureStates[arcaneBoltName]
                        && arcaneBolt.CanBeCasted
                        && arcaneBolt.CanHit(target))
                    {
                        arcaneBolt.Cast(target);
                        var castDelay = arcaneBolt.GetCastDelay(target);
                        var hitTime = arcaneBolt.GetHitTime(target) - (castDelay + 340);
                        Helpers.MultiSleeper<string>.DelaySleep($"IsHitTime_{target.Name}_{arcaneBoltName}", castDelay + 40, hitTime);
                        await Task.Delay(castDelay, token);
                        return;
                    }

                    // Dagon
                    var dagon = Abilities.Dagon;
                    if (dagon != null
                        && AutoComboMenu.ItemsSelection.PictureStates["item_dagon_5"]
                        && dagon.CanBeCasted
                        && dagon.CanHit(target))
                    {
                        dagon.Cast(target);
                        await Task.Delay(dagon.GetCastDelay(target), token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
