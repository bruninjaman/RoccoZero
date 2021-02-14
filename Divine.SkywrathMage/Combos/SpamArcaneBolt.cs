using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers;
using Divine.Core.Managers.Renderer.Particle;
using Divine.Core.Managers.TargetSelector;
using Divine.Core.Managers.Unit;
using Divine.Core.Managers.Update;
using Divine.SkywrathMage.Menus;


using Ensage.SDK.Extensions;
using Ensage.SDK.Menu.ValueBinding;

using SharpDX;

namespace Divine.SkywrathMage.Combos
{
    internal sealed class SpamArcaneBolt : BaseTaskHandler
    {
        private readonly SmartArcaneBoltMenu SmartArcaneBoltMenu;

        private readonly Abilities Abilities;

        private readonly TargetSelectorManager TargetSelector;

        private readonly UpdateHandler UpdateHandler;

        public SpamArcaneBolt(Common common)
        {
            SmartArcaneBoltMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartArcaneBoltMenu;

            Abilities = (Abilities)common.Abilities;
            TargetSelector = common.TargetSelector;

            UpdateHandler = UpdateManager.Subscribe(30, false, OnUpdate);

            SmartArcaneBoltMenu.SpamHotkeyItem.Changed += SpamHotkeyChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            SmartArcaneBoltMenu.SpamHotkeyItem.Changed -= SpamHotkeyChanged;
        }

        private void SpamHotkeyChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                TargetSelector.TargetEffectsManager.DisableTargetDraw = true;

                UpdateHandler.IsEnabled = true;

                RunAsync();
            }
            else
            {
                Cancel();

                UpdateHandler.IsEnabled = false;
                ParticleManager.Remove("SpamTarget");
                spamTarget = null;

                TargetSelector.TargetEffectsManager.DisableTargetDraw = false;
            }
        }

        private CUnit spamTarget;

        private void OnUpdate()
        {
            CUnit unitTarget = null;
            if (SmartArcaneBoltMenu.SpamUnitsItem)
            {
                unitTarget = UnitManager<CUnit, Enemy, NoIllusion>.Units.Where(x =>
                                                          x.IsVisible &&
                                                          x.IsAlive &&
                                                          x.IsSpawned &&
                                                          (x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Creep_Neutral ||
                                                          x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit ||
                                                          x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Warlock_Golem ||
                                                          x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Creep ||
                                                          x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Creep_Lane ||
                                                          x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Creep_Siege ||
                                                          x.NetworkClassId == NetworkClassId.CDOTA_Unit_Hero_Beastmaster_Boar ||
                                                          x.NetworkClassId == NetworkClassId.CDOTA_Unit_Broodmother_Spiderling ||
                                                          x.NetworkClassId == NetworkClassId.CDOTA_Unit_SpiritBear) &&
                                                          x.Distance2D(Game.MousePosition) <= 100).OrderBy(x => x.Distance2D(Game.MousePosition)).FirstOrDefault();
            }

            if (spamTarget == null || !spamTarget.IsValid || !spamTarget.IsAlive)
            {
                spamTarget = unitTarget ?? TargetSelector.Target; // TODO
            }

            if (spamTarget != null)
            {
                ParticleManager.DrawTargetLine("SpamTarget", Owner, spamTarget.Position, Color.Green);
            }
            else
            {
                ParticleManager.Remove("SpamTarget");
            }
        }

        protected override CUnit CurrentTarget
        {
            get
            {
                return spamTarget;
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
                    Orbwalker.MoveToMousePosition();
                    return;
                }

                if (!target.IsMagicImmune())
                {
                    // Arcane Bolt
                    var arcaneBolt = Abilities.ArcaneBolt;
                    if (arcaneBolt.CanBeCasted && arcaneBolt.CanHit(target))
                    {
                        arcaneBolt.Cast(target);
                        var castDelay = arcaneBolt.GetCastDelay(target);
                        var hitTime = arcaneBolt.GetHitTime(target) - (castDelay + 340);
                        MultiSleeper<string>.DelaySleep($"IsHitTime_{target.Name}_{arcaneBolt.Name}", castDelay + 40, hitTime);
                        await Task.Delay(castDelay, token);
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

        private void OrbwalkTo(CUnit target)
        {
            if (target.IsInvulnerable() || target.IsAttackImmune()) //TODO
            {
                Orbwalker.MoveToMousePosition();
                return;
            }

            switch (SmartArcaneBoltMenu.OrbwalkerItem.Value)
            {
                case "Default":
                    {
                        Orbwalker.OrbwalkTo(target);
                    }
                    break;

                case "Distance":
                    {
                        var ownerDis = Math.Min(Owner.Distance2D(Game.MousePosition), 230);
                        var ownerPos = Owner.Position.Extend(Game.MousePosition, ownerDis);
                        var pos = target.Position.Extend(ownerPos, SmartArcaneBoltMenu.MinDisInOrbwalkItem.Value);

                        Orbwalker.OrbwalkTo(target, pos);
                    }
                    break;

                case "Free":
                    {
                        var attackRange = Owner.AttackRange(target);
                        if (Owner.Distance2D(target) <= attackRange && !SmartArcaneBoltMenu.FullFreeModeItem || target.Distance2D(Game.MousePosition) <= attackRange)
                        {
                            Orbwalker.OrbwalkTo(target);
                        }
                        else
                        {
                            Orbwalker.MoveToMousePosition();
                        }
                    }
                    break;

                case "Only Attack":
                    {
                        Orbwalker.AttackTo(target);
                    }
                    break;

                case "No Move":
                    {
                        if (Owner.Distance2D(target) < Owner.AttackRange(target))
                        {
                            Orbwalker.AttackTo(target);
                        }
                    }
                    break;
            }
        }
    }
}
