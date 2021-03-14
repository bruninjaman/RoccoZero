namespace O9K.AutoUsage.Abilities.Special.Unique.SpeedBurst
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Metadata;
    using Core.Logger;

    using Divine;
    using Divine.SDK.Helpers;

    [AbilityId(AbilityId.courier_burst)]
    internal class SpeedBurstAbility
    {
        private readonly HashSet<AbilityId> ids = new HashSet<AbilityId>
        {
            AbilityId.courier_transfer_items,
            AbilityId.courier_take_stash_and_transfer_items,
            AbilityId.courier_go_to_secretshop,
        };

        public void Activate()
        {
            OrderManager.OrderAdding += this.OnOrderAdding;
        }

        public void Deactivate()
        {
            OrderManager.OrderAdding -= this.OnOrderAdding;
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
                if (order.Type != OrderType.Cast)
                {
                    return;
                }

                if (!this.ids.Contains(order.Ability.Id))
                {
                    return;
                }

                if (!(order.Units.FirstOrDefault() is Courier courier))
                {
                    return;
                }

                var burst = courier.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.courier_burst);
                if (burst == null || burst.Level == 0 || burst.Cooldown > 0)
                {
                    return;
                }

                UpdateManager.BeginInvoke(200, () => GameManager.ExecuteCommand("dota_courier_burst"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}