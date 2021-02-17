using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Divine.SkywrathMage;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.SDK.Extensions;
using Divine.SDK.Managers.Update;
using Divine.SkywrathMage.Menus;
using Divine.SkywrathMage.TargetSelector;

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
            SmartArcaneBoltMenu = common.MenuConfig.MoreMenu.SmartArcaneBoltMenu;

            Abilities = (Abilities)common.Abilities;
            TargetSelector = common.TargetSelector;

            UpdateHandler = UpdateManager.Subscribe(30, false, OnUpdate);

            SmartArcaneBoltMenu.SpamHotkeyItem.ValueChanged += SpamHotkeyChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            SmartArcaneBoltMenu.SpamHotkeyItem.ValueChanged -= SpamHotkeyChanged;
        }

        private void SpamHotkeyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
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
                ParticleManager.RemoveParticle("SpamTarget");
                spamTarget = null;

                TargetSelector.TargetEffectsManager.DisableTargetDraw = false;
            }
        }

        private Unit spamTarget;

        private void OnUpdate()
        {
            Unit unitTarget = null;
            if (SmartArcaneBoltMenu.SpamUnitsItem)
            {
                unitTarget = EntityManager.GetEntities<Unit>().Where(x =>
                            x.IsVisible &&
                            !x.IsIllusion &&
                            !x.IsAlly(Owner) &&
                            x.IsAlive &&
                            x.IsSpawned &&
                            (x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral ||
                            x.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit ||
                            x.ClassId == ClassId.CDOTA_BaseNPC_Warlock_Golem ||
                            x.ClassId == ClassId.CDOTA_BaseNPC_Creep ||
                            x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                            x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege ||
                            x.ClassId == ClassId.CDOTA_Unit_Hero_Beastmaster_Boar ||
                            x.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling ||
                            x.ClassId == ClassId.CDOTA_Unit_SpiritBear) &&
                            x.Distance2D(GameManager.MousePosition) <= 100).OrderBy(x => x.Distance2D(GameManager.MousePosition)).FirstOrDefault();
            }

            if (spamTarget == null || !spamTarget.IsValid || !spamTarget.IsAlive)
            {
                spamTarget = unitTarget ?? TargetSelector.Target; // TODO
            }

            if (spamTarget != null)
            {
                ParticleManager.TargetLineParticle("SpamTarget", Owner, spamTarget.Position, Color.Green);
            }
            else
            {
                ParticleManager.RemoveParticle("SpamTarget");
            }
        }

        protected override Unit CurrentTarget
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
                        Helpers.MultiSleeper<string>.DelaySleep($"IsHitTime_{target.Name}_{arcaneBolt.Name}", castDelay + 40, hitTime);
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

        private void OrbwalkTo(Unit target)
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
                        var ownerDis = Math.Min(Owner.Distance2D(GameManager.MousePosition), 230);
                        var ownerPos = Owner.Position.Extend(GameManager.MousePosition, ownerDis);
                        var pos = target.Position.Extend(ownerPos, SmartArcaneBoltMenu.MinDisInOrbwalkItem.Value);

                        Orbwalker.OrbwalkTo(target, pos);
                    }
                    break;

                case "Free":
                    {
                        var attackRange = Owner.AttackRange(target);
                        if (Owner.Distance2D(target) <= attackRange && !SmartArcaneBoltMenu.FullFreeModeItem || target.Distance2D(GameManager.MousePosition) <= attackRange)
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
                        Orbwalker.Attack(target);
                    }
                    break;

                case "No Move":
                    {
                        if (Owner.Distance2D(target) < Owner.AttackRange(target))
                        {
                            Orbwalker.Attack(target);
                        }
                    }
                    break;
            }
        }
    }
}
