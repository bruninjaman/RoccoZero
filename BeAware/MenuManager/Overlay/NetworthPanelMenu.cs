using Divine.Helpers;
using Divine.Menu.Items;

namespace BeAware.MenuManager.Overlay
{
    internal sealed class NetworthPanelMenu
    {
        public NetworthPanelMenu(Menu overlayMenu)
        {
            var networthPanelMenu = overlayMenu.CreateMenu("Networth Panel");
            EnableItem = networthPanelMenu.CreateSwitcher("Enable", false);
            SizeItem = networthPanelMenu.CreateSlider("Size:", 0, -20, 150);
            MoveItem = networthPanelMenu.CreateSwitcher("Move", false);
            PositionXItem = networthPanelMenu.CreateSlider("Position X:", (int)(HUDInfo.ScreenSize.X - 800), 0, 10000);
            PositionYItem = networthPanelMenu.CreateSlider("Position Y:", (int)(HUDInfo.ScreenSize.Y - 240), 0, 10000);
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSlider SizeItem { get; }

        public MenuSwitcher MoveItem { get; }

        public MenuSlider PositionXItem { get; }

        public MenuSlider PositionYItem { get; }
    }
}