namespace O9K.AutoUsage.Abilities.Autocast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu.EventArgs;

    using Divine;

    using Settings;

    internal class AutocastAbility : UsableAbility, IDisposable
    {
        private readonly UpdateHandler autocastHandler;

        private readonly GroupSettings groupSettings;

        private readonly AutocastSettings settings;

        private bool subbed;

        private Unit9 target;

        public AutocastAbility(OrbAbility orbAbility, GroupSettings settings)
            : base(orbAbility)
        {
            this.OrbAbility = orbAbility;
            this.settings = new AutocastSettings(settings.Menu, orbAbility);
            this.groupSettings = settings;

            this.autocastHandler = UpdateManager.CreateIngameUpdate(0, false, this.AutocastOnUpdate);
            this.groupSettings.GroupEnabled.ValueChange += this.EnabledOnPropertyChanged;
        }

        protected OrbAbility OrbAbility { get; }

        public void Dispose()
        {
            this.groupSettings.GroupEnabled.ValueChange -= this.EnabledOnPropertyChanged;
            OrderManager.OrderAdding -= this.OnOrderAdding;
            UpdateManager.DestroyIngameUpdate(this.autocastHandler);
        }

        public override void Enabled(bool enabled)
        {
            base.Enabled(enabled);

            if (enabled && this.groupSettings.GroupEnabled)
            {
                OrderManager.OrderAdding += this.OnOrderAdding;
                this.subbed = true;
            }
            else
            {
                OrderManager.OrderAdding -= this.OnOrderAdding;
                this.autocastHandler.IsEnabled = false;
                this.subbed = false;
            }
        }

        public override bool UseAbility(List<Unit9> heroes)
        {
            return false;
        }

        private void AutocastOnUpdate()
        {
            if (this.target?.IsValid != true || !this.target.IsAlive || this.target.IsInvulnerable || !this.target.IsVisible
                || (!this.OrbAbility.CanHitSpellImmuneEnemy && this.target.IsMagicImmune))
            {
                this.autocastHandler.IsEnabled = false;
                return;
            }

            if (this.Ability.CanBeCasted())
            {
                this.Ability.UseAbility(this.target);
            }
        }

        private void EnabledOnPropertyChanged(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue && !this.subbed && this.IsEnabled)
            {
                OrderManager.OrderAdding += this.OnOrderAdding;
                this.subbed = true;
            }
            else
            {
                OrderManager.OrderAdding -= this.OnOrderAdding;
                this.subbed = false;
            }
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (!e.Process || e.IsCustom)
                {
                    return;
                }

                var order = e.Order;
                if (order.IsQueued)
                {
                    return;
                }

                if (order.Type == OrderType.AttackTarget)
                {
                    if (this.OrbAbility.Enabled || !this.OrbAbility.CanBeCasted() || this.Owner.ManaPercentage < this.settings.MpThreshold)
                    {
                        return;
                    }

                    this.target = EntityManager9.GetUnit(order.Target.Handle);
                    if (this.target?.IsHero != true || this.target.IsIllusion || !this.settings.IsHeroEnabled(this.target.Name))
                    {
                        return;
                    }

                    if (order.Units.All(x => x.Handle != this.OwnerHandle))
                    {
                        return;
                    }

                    if (!this.groupSettings.UseWhenInvisible && !this.Owner.CanUseAbilitiesInInvisibility && this.Owner.IsInvisible)
                    {
                        return;
                    }

                    e.Process = false;
                    this.autocastHandler.IsEnabled = true;
                }
                else if (this.autocastHandler.IsEnabled)
                {
                    this.autocastHandler.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}