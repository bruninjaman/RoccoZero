using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore.Range
{
    internal sealed class CustomRangeMenu
    {
        public CustomRangeMenu(Menu rangeMenu)
        {
            var customRangeMenu = rangeMenu.CreateMenu("Custom Range");
            EnableItem = customRangeMenu.CreateSwitcher("Enable", false);
            RangeItem = customRangeMenu.CreateSlider("Range: ------------------------------------------", 500, 0, 5000);
            RedItem = customRangeMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = customRangeMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = customRangeMenu.CreateSlider("Blue:", 0, 0, 255);
        }

        public MenuSwitcher EnableItem  { get; }

        public MenuSlider RangeItem  { get; }

        public MenuSlider RedItem  { get; }

        public MenuSlider GreenItem  { get; }

        public MenuSlider BlueItem  { get; }
    }
}