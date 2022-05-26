using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BAIO.Heroes.Base;
using BAIO.Interfaces;
using BAIO.Modes;
using BAIO.UnitManager;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Orbwalker;
using Ensage.SDK.Service;
using EnsageSharp.Sandbox;
using log4net;
using Newtonsoft.Json;
using PlaySharp.Toolkit.Logging;
using SharpDX;

namespace BAIO
{
    public abstract class BaseHero : ControllableService, IHero
    {
        public Config Config;

        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Import(typeof(IServiceContext))]
        internal IServiceContext Context { get; private set; }

        internal Hero Owner { get; private set; }

        private TaskHandler KillstealHandler { get; set; }

        internal bool IsKillstealing;

        private ComboMode ComboMode { get; set; }

        private HarassMode HarassMode { get; set; }

        internal Printer Printer;

        public Updater Updater { get; set; }

        public TaskHandler UnitHandler { get; private set; }

        public TaskHandler BodyBlockHandler { get; private set; }

        public TaskHandler WardsHandler { get; private set; }



        protected abstract ComboMode GetComboMode();

        protected virtual HarassMode GetHarassMode()
        {
            return new HarassMode(this);
        }

        protected virtual void InventoryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        protected virtual async Task KillStealAsync(CancellationToken token)
        {
            // ¯\_(ツ)_/¯
        }

        protected void KillstealPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Config.General.Killsteal)
            {
                this.KillstealHandler.RunAsync();
            }
            else
            {
                this.KillstealHandler.Cancel();
            }
        }

        public async Task AwaitKillstealDelay(int delay, CancellationToken token = default(CancellationToken))
        {
            this.IsKillstealing = true;
            try
            {
                await Task.Delay(delay, token);
            }
            finally
            {
                this.IsKillstealing = false;
            }
        }

        protected virtual void TargetIndicatorUpdater()
        {
            if ((this.ComboMode == null) || !this.Config.General.DrawTargetIndicator)
            {
                return;
            }

            if (this.ComboMode.CanExecute && (this.ComboMode.CurrentTarget != null))
            {
                this.Context.Particle.DrawTargetLine(this.Owner, "TargetIndicator", this.ComboMode.CurrentTarget.NetworkPosition);
            }
            else
            {
                this.Context.Particle.Remove("TargetIndicator");
            }
        }

        private void DrawTargetLinePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!this.Config.General.DrawTargetIndicator)
            {
                this.Context.Particle.Remove("TargetIndicator");
            }
        }

        private void NoobFailSafe()
        {
            var orbwalker = this.Context.Orbwalker;
            if (!orbwalker.IsActive)
            {
                orbwalker.Activate();
            }

            var targetSelector = this.Context.TargetSelector;
            if (!targetSelector.IsActive)
            {
                targetSelector.Activate();
            }

            var prediction = this.Context.Prediction;
            if (!prediction.IsActive)
            {
                prediction.Activate();
            }
        }

        private async Task Bodyblock(CancellationToken token)
        {
            if (Game.IsPaused || !this.Config.General.ComboKey.Value.Active || this.ComboMode.CurrentTarget == null)
            {
                await Task.Delay(250, token);
                return;
            }

            if (!this.Config.General.Enabled)
            {
                return;
            }

            if (this.Updater.AllUnits.Any() && this.ComboMode.CurrentTarget != null && this.Config.Hero.Enabled &&
                this.Updater.AllUnits.First().Unit.Name != "npc_dota_templar_assassin_psionic_trap")
            {
                BodyBlockerUnit = this.Updater.AllUnits.OrderBy(x => x.Unit.Distance2D(this.ComboMode.CurrentTarget)).First().Unit;
            }
            else
            {
                BodyBlockerUnit = null;
            }

            if (BodyBlockerUnit != null && this.Config.Hero.Enabled)
            {
                var angle = Ensage.SDK.Extensions.UnitExtensions.FindRotationAngle(this.ComboMode.CurrentTarget, this.BodyBlockerUnit.Position);

                if (angle > 1.3)
                {
                    var delta = angle * 0.6f;
                    var position = this.ComboMode.CurrentTarget.NetworkPosition;
                    var side1 = position + this.ComboMode.CurrentTarget.Vector3FromPolarAngle(delta)
                                * Math.Max(this.Config.Hero.BlockSensitivity.Value, 150);
                    var side2 = position + this.ComboMode.CurrentTarget.Vector3FromPolarAngle(-delta)
                                * Math.Max(this.Config.Hero.BlockSensitivity.Value, 150);
                    this.BodyBlockerUnit.Move(side1.Distance(this.BodyBlockerUnit.Position) < side2.Distance(this.BodyBlockerUnit.Position) ? side1 : side2);
                }
                else
                {
                    if (this.ComboMode.CurrentTarget.IsMoving && angle < 0.3 && this.BodyBlockerUnit.IsMoving)
                    {
                        this.BodyBlockerUnit.Stop();
                        return;
                    }

                    if (this.BodyBlockerUnit.Distance2D(this.ComboMode.CurrentTarget.InFront(this.Config.Hero.BlockSensitivity)) >
                        50)
                    {
                        this.BodyBlockerUnit.Move(this.ComboMode.CurrentTarget.InFront(this.Config.Hero.BlockSensitivity));
                    }
                    else
                    {
                        this.BodyBlockerUnit.Attack(this.ComboMode.CurrentTarget);
                        return;
                    }
                }
            }

            await Task.Delay(50, token);
        }

        public Unit BodyBlockerUnit { get; set; }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Config.General.ComboKey.Value.Active)
            {
                await Task.Delay(250, token);
                return;
            }

            if (!this.Config.General.Enabled)
            {
                return;
            }

            foreach (var unit in this.Updater.AllUnits)
            {
                if (unit.Unit != BodyBlockerUnit && this.Config.Hero.ControlUnits && unit.Unit.Name != "npc_dota_templar_assassin_psionic_trap")
                {
                    if (this.ComboMode.CurrentTarget == null || UnitExtensions.IsInvul(this.ComboMode.CurrentTarget) ||
                        this.ComboMode.CurrentTarget.IsAttackImmune())
                    {
                        Player.AttackEntity(unit.Unit, (Unit) null);
                        //unit.UnitMovementManager.Orbwalk(null);
                    }
                    else
                    {
                        Player.AttackEntity(unit.Unit, this.ComboMode.CurrentTarget);
                        //unit.UnitMovementManager.Orbwalk(this.ComboMode.CurrentTarget);
                    }
                }

                var ability1 = unit.Ability;
                var ability2 = unit.Ability2;
                var ability3 = unit.Ability3;
                var ability4 = unit.Ability4;

                if (this.ComboMode.CurrentTarget != null && this.Config.Hero.UseUnitAbilities &&
                    !UnitExtensions.IsInvul(this.ComboMode.CurrentTarget) && !this.ComboMode.CurrentTarget.IsAttackImmune() && !this.ComboMode.CurrentTarget.IsMagicImmune() &&
                        (ability1 != null && AbilityExtensions.CanBeCasted(ability1) ||
                         ability2 != null && AbilityExtensions.CanBeCasted(ability2) ||
                         ability3 != null && AbilityExtensions.CanBeCasted(ability3) ||
                         ability4 != null && AbilityExtensions.CanBeCasted(ability4)))
                {
                    if (ability1 != null
                        && AbilityExtensions.CanBeCasted(ability1)
                        && UnitCastingChecks(ability1.Name, unit.Unit, this.ComboMode.CurrentTarget, ability1)
                        && (ability1.TargetTeamType == TargetTeamType.Enemy ||
                            ability1.TargetTeamType == TargetTeamType.None)
                        && (unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityCastRange(unit.Unit, ability1) ||
                            unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityRadius(unit.Unit, ability1) - 70))
                    {
                        if (ability1.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (ability1.UseAbility())
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability1.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                        {
                            if (ability1.UseAbility(this.ComboMode.CurrentTarget))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability1.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                        {
                            if (ability1.UseAbility(this.ComboMode.CurrentTarget.Position))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                    }
                    else if (ability1 != null
                             && AbilityExtensions.CanBeCasted(ability1)
                             && ability1.TargetTeamType == TargetTeamType.Allied
                             && unit.Unit.Distance2D(this.Owner) <= Extensions.GetAbilityCastRange(unit.Unit, ability1))
                    {
                        if (ability1.UseAbility(this.Owner))
                        {
                            await Task.Delay(180, token);
                        }
                    }

                    if (ability2 != null
                        && AbilityExtensions.CanBeCasted(ability2)
                        && UnitCastingChecks(ability2.Name, unit.Unit, this.ComboMode.CurrentTarget, ability2)
                        && AbilityExtensions.CanHit(ability2, this.ComboMode.CurrentTarget)
                        && (ability2.TargetTeamType == TargetTeamType.Enemy ||
                            ability2.TargetTeamType == TargetTeamType.None)
                        && (unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityCastRange(unit.Unit, ability2) ||
                            unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityRadius(unit.Unit, ability2) - 70))
                    {
                        if (ability2.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (ability2.UseAbility())
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability2.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                        {
                            if (ability2.UseAbility(this.ComboMode.CurrentTarget))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability2.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                        {
                            if (ability2.UseAbility(this.ComboMode.CurrentTarget.Position))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                    }
                    else if (ability2 != null
                             && AbilityExtensions.CanBeCasted(ability2)
                             && ability2.TargetTeamType == TargetTeamType.Allied
                             && unit.Unit.Distance2D(this.Owner) <= Extensions.GetAbilityCastRange(unit.Unit, ability2))
                    {
                        if (ability2.UseAbility(this.Owner))
                        {
                            await Task.Delay(180, token);
                        }
                    }

                    if (ability3 != null
                        && AbilityExtensions.CanBeCasted(ability3)
                        && UnitCastingChecks(ability3.Name, unit.Unit, this.ComboMode.CurrentTarget, ability3)
                        && AbilityExtensions.CanHit(ability3, this.ComboMode.CurrentTarget)
                        && (ability3.TargetTeamType == TargetTeamType.Enemy ||
                            ability3.TargetTeamType == TargetTeamType.None)
                        && (unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityCastRange(unit.Unit, ability3) ||
                            unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityRadius(unit.Unit, ability3) - 70))
                    {
                        if (ability3.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (ability3.UseAbility())
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability3.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                        {
                            if(ability3.UseAbility(this.ComboMode.CurrentTarget))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability3.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                        {
                            if(ability3.UseAbility(this.ComboMode.CurrentTarget.Position))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                    }
                    else if (ability3 != null
                             && AbilityExtensions.CanBeCasted(ability3)
                             && ability3.TargetTeamType == TargetTeamType.Allied
                             && unit.Unit.Distance2D(this.Owner) <= Extensions.GetAbilityCastRange(unit.Unit, ability3))
                    {
                        if (ability3.UseAbility(this.Owner))
                        {
                            await Task.Delay(180, token);
                        }
                    }

                    if (ability4 != null
                        && AbilityExtensions.CanBeCasted(ability4)
                        && UnitCastingChecks(ability4.Name, unit.Unit, this.ComboMode.CurrentTarget, ability4)
                        && AbilityExtensions.CanHit(ability4, this.ComboMode.CurrentTarget)
                        && (ability4.TargetTeamType == TargetTeamType.Enemy ||
                            ability4.TargetTeamType == TargetTeamType.None)
                        && (unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityCastRange(unit.Unit, ability4) ||
                            unit.Unit.Distance2D(this.ComboMode.CurrentTarget) <= Extensions.GetAbilityRadius(unit.Unit, ability4) - 70))
                    {
                        if (ability4.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (ability4.UseAbility())
                            {
                                await Task.Delay(180, token);

                            }
                        }
                        else if (ability4.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                        {
                            if (ability4.UseAbility(this.ComboMode.CurrentTarget))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                        else if (ability4.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                        {
                            if (ability4.UseAbility(this.ComboMode.CurrentTarget.Position))
                            {
                                await Task.Delay(180, token);
                            }
                        }
                    }
                    else if (ability4 != null
                             && AbilityExtensions.CanBeCasted(ability4)
                             && ability4.TargetTeamType == TargetTeamType.Allied
                             && unit.Unit.Distance2D(this.Owner) <= Extensions.GetAbilityCastRange(unit.Unit, ability4))
                    {
                        if (ability4.UseAbility(this.Owner))
                        {
                            await Task.Delay(180, token);
                        }
                    }
                }
            }

            await Task.Delay(100, token);
        }
        private int WardsRange => this.Owner.AghanimState() ? 875 : 650;

        public virtual async Task WardsAttack(CancellationToken tk)
        {
            if (this.ComboMode.CurrentTarget == null)
            {
                return;
            }

            var wardsShouldAttack = EntityManager<Unit>.Entities.Where(x =>
                x != null && !Game.IsPaused && x.IsValid && x.Distance2D(this.ComboMode.CurrentTarget) <= WardsRange &&
                x.Name.Contains("npc_dota_shadow_shaman_ward") &&
                Ensage.SDK.Extensions.UnitExtensions.CanAttack(x, this.ComboMode.CurrentTarget)).ToList();

            if (!wardsShouldAttack.Any())
            {
                return;
            }

            if (Player.EntitiesAttack(wardsShouldAttack, this.ComboMode.CurrentTarget))
            {
                await Task.Delay(200, tk);
            }

            await Task.Delay(100, tk);
            //foreach (var ward in wardsShouldAttack)
            //{
            //    if (ward == null || Game.IsPaused || !ward.IsValid ||
            //        !Ensage.SDK.Extensions.UnitExtensions.CanAttack(ward, this.ComboMode.CurrentTarget) ||
            //        ward.Distance2D(this.ComboMode.CurrentTarget) >= WardsRange) continue;

            //    if (ward.Attack(this.ComboMode.CurrentTarget))
            //    {
            //        await Task.Delay(200, tk);
            //    }
            //}
        }

        public bool UnitCastingChecks(string name, Unit hero, Unit target, Ability ability = null)
        {
            if (name == "lone_druid_savage_roar_bear" && ability != null && !target.HasModifier("modifier_lone_druid_spirit_bear_entangle_effect"))
            {
                return false;
            }

            if (name == "templar_assassin_self_trap" && ability != null &&
                target.HasModifier("modifier_templar_assassin_trap_slow") && target.Distance2D(hero) <= 250)
            {
                return false;
            }

            return true;
        }

        protected override void OnActivate()
        {
            try
            {
                this.Context.Inventory.Attach(this);
                this.Owner = (Hero) this.Context.Owner;

                NoobFailSafe();

                this.Context.Orbwalker.Settings.Attack.Value = true;
                this.Context.Orbwalker.Settings.Move.Value = true;

                this.Config = new Config(this.Owner.HeroId);
                this.ComboMode = this.GetComboMode();
                this.Context.Orbwalker.RegisterMode(this.ComboMode);
                this.HarassMode = this.GetHarassMode();
                this.Context.Orbwalker.RegisterMode(this.HarassMode);

                this.KillstealHandler = UpdateManager.Run(this.KillStealAsync, true, this.Config.General.Killsteal);

                UpdateManager.Subscribe(this.TargetIndicatorUpdater);

                this.Config.General.DrawTargetIndicator.PropertyChanged += this.DrawTargetLinePropertyChanged;
                this.Config.General.Killsteal.PropertyChanged += this.KillstealPropertyChanged;
                this.Context.Inventory.CollectionChanged += this.InventoryChanged;

                this.Updater = new Updater(this);
                this.UnitHandler = UpdateManager.Run(OnUpdate);
                this.BodyBlockHandler = UpdateManager.Run(Bodyblock);

                if (this.Owner.HeroId == HeroId.npc_dota_hero_shadow_shaman)
                {
                    this.WardsHandler = UpdateManager.Run(WardsAttack);
                }

                Printer.Print($"Thanks for purchasing/trialing my assembly!!");
                Printer.Print($"If you encounter any bugs/errors please report them over forum");
                Printer.Print($"Have fun! -beminee");
            }
            catch (TaskCanceledException)
            {
                // ignore.
            }
            catch (NullReferenceException)
            {
                Printer.Print($"Please enable orbwalker in Ensage.SDK > Plugins menu.");
            }
            catch (Exception e)
            {
                Log.Error($"Exception in OnActivate: {e}");
            }
        }

        protected override void OnDeactivate()
        {
            this.Context.Inventory.Detach(this);

            this.Context.Orbwalker.UnregisterMode(this.HarassMode);
            this.Context.Orbwalker.UnregisterMode(this.ComboMode);

            this.Config.General.DrawTargetIndicator.PropertyChanged -= this.DrawTargetLinePropertyChanged;
            this.Config.General.Killsteal.PropertyChanged -= this.KillstealPropertyChanged;
            this.Context.Inventory.CollectionChanged -= this.InventoryChanged;

            UpdateManager.Unsubscribe(this.TargetIndicatorUpdater);
            this.UnitHandler?.Cancel();
            this.BodyBlockHandler?.Cancel();
            this.KillstealHandler.Cancel();

            this.Config.Dispose();
            
        }
    }
}
