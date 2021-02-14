using System;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.TargetSelector;
using Divine.SkywrathMage.Menus;
using Divine.SkywrathMage.Menus.Combo;


using Ensage.SDK.Extensions;

namespace Divine.SkywrathMage.Combos
{
    internal sealed class Combo : BaseCombo
    {
        private readonly BaseSpellsMenu SpellsMenu;

        private readonly BaseItemsMenu ItemsMenu;

        private readonly MysticFlareMenu MysticFlareMenu;

        private readonly BaseBlinkDaggerMenu BlinkDaggerMenu;

        private readonly BaseAeonDiskMenu AeonDiskMenu;

        private readonly BaseWithMuteMenu WithMuteMenu;

        private readonly SmartConcussiveShotMenu SmartConcussiveShotMenu;

        private readonly BaseBladeMailMenu BladeMailMenu;

        private readonly Abilities Abilities;

        private readonly TargetSelectorManager TargetSelector;

        private readonly BaseLinkenBreaker LinkenBreaker;

        public Combo(Common common)
            : base(common.MenuConfig)
        {
            SpellsMenu = common.MenuConfig.ComboMenu.SpellsMenu;
            ItemsMenu = common.MenuConfig.ComboMenu.ItemsMenu;
            MysticFlareMenu = ((ComboMenu)common.MenuConfig.ComboMenu).MysticFlareMenu;
            BlinkDaggerMenu = common.MenuConfig.ComboMenu.BlinkDaggerMenu;
            AeonDiskMenu = common.MenuConfig.ComboMenu.AeonDiskMenu;
            WithMuteMenu = common.MenuConfig.ComboMenu.WithMuteMenu;
            SmartConcussiveShotMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartConcussiveShotMenu;
            BladeMailMenu = common.MenuConfig.BladeMailMenu;

            Abilities = (Abilities)common.Abilities;
            TargetSelector = common.TargetSelector;
            LinkenBreaker = common.LinkenBreaker;
        }

        protected override CUnit CurrentTarget
        {
            get
            {
                return TargetSelector.Target;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (IsStopped)
                {
                    return;
                }

                var target = CurrentTarget;
                if (IsNullTarget(target))
                {
                    MoveToMousePosition();
                    return;
                }

                if (Owner.HasModifier("modifier_item_silver_edge_windwalk"))
                {
                    OrbwalkTo(target);

                    if (!Owner.CanMove())
                    {
                        await Task.Delay((int)(Owner.GetAutoAttackArrivalTime(target) * 1000), token);
                    }

                    return;
                }

                var targetModifiers = new TargetModifiers(target);
                if (BladeMailMenu.BladeMailItem && targetModifiers.ModifierReflect != null)
                {
                    MoveToMousePosition();
                    return;
                }

                // Blink
                var blink = Abilities.Blink;
                if (blink != null
                    && ItemsMenu.ItemsSelection[blink.Name]
                    && Owner.Distance2D(Game.MousePosition) > BlinkDaggerMenu.BlinkDistanceEnemyItem.Value
                    && Owner.Distance2D(target) > 600
                    && blink.CanBeCasted)
                {
                    var blinkPos = target.Position.Extend(Game.MousePosition, BlinkDaggerMenu.BlinkDistanceEnemyItem.Value);
                    if (Owner.Distance2D(blinkPos) < blink.CastRange)
                    {
                        blink.Cast(blinkPos);
                        await Task.Delay(blink.GetCastDelay(blinkPos), token);
                    }
                }

                if (!target.IsBlockMagicDamage(targetModifiers) && WithMute(target))
                {
                    if (!target.IsShieldAbilities(targetModifiers) || targetModifiers.IsSilverDebuff)
                    {
                        var comboBreaker = !AeonDiskMenu.EnableItem || target.ComboBreaker();

                        // Hex
                        var modifierStun = targetModifiers.ModifierStun;
                        var modifierHex = targetModifiers.ModifierHex;
                        var hex = Abilities.Hex;
                        if (hex != null
                            && ItemsMenu.ItemsSelection[hex.Name]
                            && !comboBreaker
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
                            && ItemsMenu.ItemsSelection[orchid.Name]
                            && !comboBreaker
                            && orchid.CanBeCasted
                            && orchid.CanHit(target))
                        {
                            orchid.Cast(target);
                            await Task.Delay(orchid.GetCastDelay(target), token);
                        }

                        // Bloodthorn
                        var bloodthorn = Abilities.Bloodthorn;
                        if (bloodthorn != null
                            && ItemsMenu.ItemsSelection[bloodthorn.Name]
                            && !comboBreaker
                            && bloodthorn.CanBeCasted
                            && bloodthorn.CanHit(target))
                        {
                            bloodthorn.Cast(target);
                            await Task.Delay(bloodthorn.GetCastDelay(target), token);
                        }

                        // MysticFlare
                        var mysticFlare = Abilities.MysticFlare;
                        if (SpellsMenu.SpellsSelection[mysticFlare.Name]
                            && !comboBreaker
                            && MysticFlareMenu.MinHealthToUltItem.Value <= ((float)target.Health / target.MaximumHealth) * 100
                            && mysticFlare.CanBeCasted
                            && mysticFlare.CanHit(target)
                            && (BadUlt(target) || Utils.AutoCombo(target)))
                        {
                            var position = mysticFlare.MysticFlarePrediction(Prediction, target);
                            mysticFlare.Cast(position);
                            await Task.Delay(mysticFlare.GetCastDelay(position), token);
                        }

                        // Nullifier
                        var nullifier = Abilities.Nullifier;
                        if (nullifier != null
                            && ItemsMenu.ItemsSelection[nullifier.Name]
                            && !comboBreaker
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
                            && ItemsMenu.ItemsSelection[atos.Name]
                            && !comboBreaker
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
                        if (SpellsMenu.SpellsSelection[ancientSeal.Name]
                            && !comboBreaker
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
                            && ItemsMenu.ItemsSelection[veil.Name]
                            && veil.CanBeCasted
                            && veil.CanHit(target))
                        {
                            veil.Cast(target.Position);
                            await Task.Delay(veil.GetCastDelay(target.Position), token);
                        }

                        // Ethereal
                        var ethereal = Abilities.Ethereal;
                        if (ethereal != null
                            && ItemsMenu.ItemsSelection[ethereal.Name]
                            && !comboBreaker
                            && ethereal.CanBeCasted
                            && ethereal.CanHit(target))
                        {
                            ethereal.Cast(target);
                            MultiSleeper<string>.Sleep($"IsHitTime_{target.Name}_{ethereal.Name}", ethereal.GetHitTime(target));
                            await Task.Delay(ethereal.GetCastDelay(target), token);
                            return;
                        }

                        // Shivas
                        var shivas = Abilities.Shivas;
                        if (shivas != null
                            && ItemsMenu.ItemsSelection[shivas.Name]
                            && shivas.CanBeCasted
                            && shivas.CanHit(target))
                        {
                            shivas.Cast();
                            await Task.Delay(shivas.GetCastDelay(), token);
                        }

                        if (!MultiSleeper<string>.Sleeping($"IsHitTime_{target.Name}_item_ethereal_blade") || targetModifiers.IsEthereal)
                        {
                            // ConcussiveShot
                            var concussiveShot = Abilities.ConcussiveShot;
                            if (SpellsMenu.SpellsSelection[concussiveShot.Name]
                                && concussiveShot.CanBeCasted
                                && concussiveShot.CanHit(target)
                                && Utils.ConcussiveShotTarget(SmartConcussiveShotMenu, target, concussiveShot.TargetHit))
                            {
                                concussiveShot.Cast();
                                await Task.Delay(concussiveShot.GetCastDelay(), token);
                            }

                            // ArcaneBolt
                            var arcaneBolt = Abilities.ArcaneBolt;
                            if (SpellsMenu.SpellsSelection[arcaneBolt.Name]
                                && arcaneBolt.CanBeCasted
                                && arcaneBolt.CanHit(target))
                            {
                                arcaneBolt.Cast(target);
                                var castDelay = arcaneBolt.GetCastDelay(target);
                                var hitTime = arcaneBolt.GetHitTime(target) - (castDelay + 350);
                                MultiSleeper<string>.DelaySleep($"IsHitTime_{target.Name}_{arcaneBolt.Name}", castDelay + 50, hitTime);
                                await Task.Delay(castDelay, token);
                                return;
                            }

                            // Dagon
                            var dagon = Abilities.Dagon;
                            if (dagon != null
                                && ItemsMenu.ItemsSelection["item_dagon_5"]
                                && !comboBreaker
                                && dagon.CanBeCasted
                                && dagon.CanHit(target))
                            {
                                dagon.Cast(target);
                                await Task.Delay(dagon.GetCastDelay(target), token);
                                return;
                            }
                        }

                        // Urn
                        var urn = Abilities.Urn;
                        if (urn != null
                            && ItemsMenu.ItemsSelection[urn.Name]
                            && !comboBreaker
                            && urn.CanBeCasted
                            && urn.CanHit(target))
                        {
                            urn.Cast(target);
                            await Task.Delay(urn.GetCastDelay(target), token);
                        }

                        // Vessel
                        var vessel = Abilities.Vessel;
                        if (vessel != null
                            && ItemsMenu.ItemsSelection[vessel.Name]
                            && !comboBreaker
                            && vessel.CanBeCasted
                            && vessel.CanHit(target))
                        {
                            vessel.Cast(target);
                            await Task.Delay(vessel.GetCastDelay(target), token);
                        }
                    }
                    else
                    {
                        LinkenBreaker.RunAsync();
                    }
                }

                OrbwalkTo(target);
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

        protected override bool WithMute(CUnit target)
        {
            if (!WithMuteMenu.ToggleHotkeyItem)
            {
                return true;
            }

            var hex = Abilities.Hex;
            var ancientSeal = Abilities.AncientSeal;

            if (hex != null && ItemsMenu.ItemsSelection[hex.Name] && hex.CanBeCasted && hex.CanHit(target))
            {
                return true;
            }

            if (SpellsMenu.SpellsSelection[ancientSeal.Name] && ancientSeal.CanBeCasted && !ancientSeal.CanHit(target))
            {
                return false;
            }

            return true;
        }

        private bool BadUlt(CUnit target)
        {
            if (!MysticFlareMenu.BadUltItem)
            {
                return false;
            }

            if (Abilities.Atos != null || Abilities.Hex != null || Abilities.Ethereal != null)
            {
                return false;
            }

            if (target.MovementSpeed < MysticFlareMenu.BadUltMovementSpeedItem.Value)
            {
                return true;
            }

            return false;
        }
    }
}
