namespace O9K.AutoUsage.Abilities.Buff.Unique.PhaseBoots
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base.Types;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Logger;

    using Divine;

    using Settings;

    [AbilityId(AbilityId.item_phase_boots)]
    internal class PhaseBootsAbility : BuffAbility, IDisposable
    {
        private readonly PhaseBootsSettings settings;

        public PhaseBootsAbility(IBuff ability, GroupSettings settings)
            : base(ability)
        {
            this.settings = new PhaseBootsSettings(settings.Menu, ability);
        }

        public void Dispose()
        {
            OrderManager.OrderAdding -= this.OnOrderAdding;
        }

        public override void Enabled(bool enabled)
        {
            base.Enabled(enabled);

            if (enabled)
            {
                OrderManager.OrderAdding += this.OnOrderAdding;
            }
            else
            {
                OrderManager.OrderAdding -= this.OnOrderAdding;
            }
        }

        public override bool UseAbility(List<Unit9> heroes)
        {
            return false;
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (!e.Process)
                {
                    return;
                }

                var order = e.Order;
                if (order.IsQueued || !this.Ability.CanBeCasted())
                {
                    return;
                }

                if (order.Units.All(x => x.Handle != this.OwnerHandle))
                {
                    return;
                }

                if (!this.Owner.CanMove() || (!this.Owner.CanUseAbilitiesInInvisibility && this.Owner.IsInvisible))
                {
                    return;
                }

                switch (order.Type)
                {
                    case OrderType.AttackPosition:
                    case OrderType.AttackTarget:
                        {
                            var location = order.Target?.Position ?? order.Position;
                            if (this.Owner.Distance(location) - this.Owner.GetAttackRange() >= this.settings.Distance)
                            {
                                this.Ability.UseAbility();
                            }

                            break;
                        }
                    case OrderType.MoveTarget:
                    case OrderType.MovePosition:
                        {
                            var location = order.Target?.Position ?? order.Position;
                            if (this.Owner.Distance(location) >= this.settings.Distance)
                            {
                                this.Ability.UseAbility();
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}