using Divine.Menu;
using Divine.Menu.Items;

namespace Divine.Humanizer
{
    internal sealed class Menu
    {
        public Menu()
        {
            var rootMenu = MenuManager.CreateRootMenu("Divine.Humanizer");
            var debuggerMenu = rootMenu.CreateMenu("Debugger");
            DrawSwitcher = debuggerMenu.CreateSwitcher("Draw", false);
            TestSwitcher = debuggerMenu.CreateSwitcher("Test", false);

            TriggerSwitcher = rootMenu.CreateSlider("Trigger", 0, 0, 100);
            TriggerSwitcher.IsHidden = true;

            var orderMenu = rootMenu.CreateMenu("Order");
            orderMenu.IsHidden = true;

            OrderEnableSwitcher = orderMenu.CreateSwitcher("Enable", true);
            OrderRateSlider = orderMenu.CreateSlider("Rate", 50, 50, 1000);
        }

        public MenuSwitcher DrawSwitcher { get; }

        public MenuSwitcher TestSwitcher { get; }

        public MenuSlider TriggerSwitcher { get; }

        public MenuSwitcher OrderEnableSwitcher { get; }

        public MenuSlider OrderRateSlider { get; }
    }
}