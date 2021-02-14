using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class TextPanelMenu
    {
        public TextPanelMenu(Menu.Items.Menu menu)
        {
            var textPanelMenu = menu.CreateMenu("Text Panel");
            ComboPanelItem = textPanelMenu.CreateSwitcher("Combo Panel");
            UnitComboPanelItem = textPanelMenu.CreateSwitcher("Unit Combo Panel");
            MoveItem = textPanelMenu.CreateSwitcher("Move", false);
            PositionX = textPanelMenu.CreateSlider("Position X:", (int)RendererManager.ScreenSize.X - 200, 0, 10000);
            PositionY = textPanelMenu.CreateSlider("Position Y:", (int)RendererManager.ScreenSize.Y - 310, 0, 10000);
        }

        public MenuSwitcher ComboPanelItem { get; }

        public MenuSwitcher UnitComboPanelItem { get; }

        public MenuSwitcher MoveItem { get; }

        public MenuSlider PositionX { get; }

        public MenuSlider PositionY { get; }
    }
}