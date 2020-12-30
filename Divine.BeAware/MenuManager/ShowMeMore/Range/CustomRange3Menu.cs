using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore.Range
{
    internal sealed class CustomRange3Menu
    {
        public CustomRange3Menu(Menu.Items.Menu rangeMenu)
        {
            var customRange3Menu = rangeMenu.CreateMenu("Custom Range 3");
            EnableItem = customRange3Menu.CreateSwitcher("Enable", false);
            RangeItem = customRange3Menu.CreateSlider("Range: ------------------------------------------", 700, 0, 5000);
            RedItem = customRange3Menu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = customRange3Menu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = customRange3Menu.CreateSlider("Blue:", 0, 0, 255);
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSlider RangeItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }
    }
}