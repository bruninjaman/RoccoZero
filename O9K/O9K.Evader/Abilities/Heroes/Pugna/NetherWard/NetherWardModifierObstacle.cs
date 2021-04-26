namespace O9K.Evader.Abilities.Heroes.Pugna.NetherWard
{
    using System;

    using Base;

    using Core.Entities.Abilities.Heroes.Pugna;
    using Core.Entities.Units;
    using Core.Logger;
    using Core.Managers.Entity;

    using Divine;

    using Pathfinder.Obstacles.Modifiers;

    internal class NetherWardModifierObstacle : ModifierAllyObstacle, IDisposable
    {
        private readonly NetherWard netherWard;

        public NetherWardModifierObstacle(IModifierCounter ability, Modifier modifier, Unit9 modifierOwner)
            : base(ability, modifier, modifierOwner)
        {
            if (!modifierOwner.IsControllable)
            {
                return;
            }

            this.netherWard = (NetherWard)this.EvadableAbility.Ability;

            OrderManager.OrderAdding += this.OnOrderAdding;
        }

        public void Dispose()
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
                var orderType = order.Type;
                if (orderType != OrderType.Cast && orderType != OrderType.CastPosition && orderType != OrderType.CastTarget)
                {
                    return;
                }

                var ability = EntityManager9.GetAbility(order.Ability.Handle);
                if (ability == null)
                {
                    return;
                }

                var owner = ability.Owner;
                if (!owner.Equals(this.ModifierOwner))
                {
                    return;
                }

                var damage = this.netherWard.GetDamage(owner, ability.ManaCost);
                if (damage <= 0)
                {
                    return;
                }

                if (damage > 300 || owner.Health - damage <= 0)
                {
                    e.Process = false;
                }
            }
            catch (Exception ex)
            {
                OrderManager.OrderAdding -= this.OnOrderAdding;
                Logger.Error(ex);
            }
        }
    }
}