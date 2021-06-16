using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore.Range
{
    internal sealed class CustomRange2Menu
    {
        public CustomRange2Menu(Menu rangeMenu)
        {
            var customRange2Menu = rangeMenu.CreateMenu("Custom Range 2");
            EnableItem = customRange2Menu.CreateSwitcher("Enable", false);
            RangeItem = customRange2Menu.CreateSlider("Range: ------------------------------------------", 600, 0, 5000);
            RedItem = customRange2Menu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = customRange2Menu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = customRange2Menu.CreateSlider("Blue:", 0, 0, 255);
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSlider RangeItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }
    }
}