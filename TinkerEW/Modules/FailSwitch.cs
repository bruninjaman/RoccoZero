using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Order;

using System.Linq;

namespace TinkerEW.Modules
{
    internal sealed class FailSwitch
    {
        private readonly Menu Menu;

        public FailSwitch(Menu menu)
        {
            Menu = menu;
            OrderManager.OrderAdding += OrderManager_OrderAdding;
        }

        private void OrderManager_OrderAdding(Divine.Order.EventArgs.OrderAddingEventArgs e)
        {
            if (e.IsCustom
                || (e.Order.Ability?.Id != AbilityId.tinker_heat_seeking_missile
                && e.Order.Ability?.Id != AbilityId.tinker_rearm))
            {
                return;
            }

            System.Console.WriteLine(EntityManager.GetEntities<Hero>().Any(x => x.IsAlive && x.IsVisible && !x.IsAlly(EntityManager.LocalHero) && x.Distance2D(EntityManager.LocalHero) <= EntityManager.LocalHero.GetAbilityById(AbilityId.tinker_heat_seeking_missile).CastRange));
            if (e.Order.Ability?.Id == AbilityId.tinker_heat_seeking_missile
                && !EntityManager.GetEntities<Hero>().Any(x => x.IsAlive
                && x.IsVisible
                && !x.IsAlly(EntityManager.LocalHero)
                && x.Distance2D(EntityManager.LocalHero) <= EntityManager.LocalHero.GetAbilityById(AbilityId.tinker_heat_seeking_missile).CastRange))
            {
                e.Process = false;
                return;
            }

        }

        public void Dispose()
        {
            OrderManager.OrderAdding -= OrderManager_OrderAdding;
        }
    }
}
