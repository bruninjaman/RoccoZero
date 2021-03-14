namespace O9K.ItemManager.Modules.AutoActions
{
    using System;

    using Core.Entities.Abilities.Base;
    using Core.Extensions;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine;
    using Divine.SDK.Extensions;
    using Divine.SDK.Localization;

    using Metadata;

    internal class BlinkRangeChange : IModule
    {
        private readonly MenuSwitcher enabled;

        public BlinkRangeChange(IMainMenu mainMenu)
        {
            var menu = mainMenu.AutoActionsMenu.Add(new Menu(LocalizationHelper.LocalizeName(AbilityId.item_blink), "BlinkDagger"));

            this.enabled = menu.Add(new MenuSwitcher("Maximize blink range"));
            this.enabled.AddTranslation(Lang.Ru, "Максимизировать дальность блинка");
            this.enabled.AddTranslation(Lang.Cn, "最大化眨眼范围");
        }

        public void Activate()
        {
            this.enabled.ValueChange += this.OnValueChange;
        }

        public void Dispose()
        {
            this.enabled.ValueChange -= this.OnValueChange;
            OrderManager.OrderAdding -= this.OnOrderAdding;
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (e.IsCustom || !e.Process)
                {
                    return;
                }

                var order = e.Order;
                if (order.IsQueued)
                {
                    return;
                }

                if (order.Type != OrderType.CastPosition || order.Ability.Id != AbilityId.item_blink)
                {
                    return;
                }

                var blink = (ActiveAbility)EntityManager9.GetAbility(order.Ability.Handle);
                var hero = blink.Owner;

                if (hero.IsChanneling)
                {
                    return;
                }

                var blinkRange = blink.Range;
                var blinkPosition = order.Position;
                var heroPosition = hero.Position;
                if (heroPosition.Distance2D(blinkPosition) < blinkRange)
                {
                    return;
                }

                var newBlinkPosition = heroPosition.Extend2D(blinkPosition, blinkRange - 50);
                if (!Hud.IsPositionOnScreen(newBlinkPosition))
                {
                    return;
                }

                blink.UseAbility(newBlinkPosition);
                e.Process = false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                OrderManager.OrderAdding += this.OnOrderAdding;
            }
            else
            {
                OrderManager.OrderAdding -= this.OnOrderAdding;
            }
        }
    }
}