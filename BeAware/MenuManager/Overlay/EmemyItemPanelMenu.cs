namespace BeAware.MenuManager.Overlay;

using Divine.Helpers;
using Divine.Menu.Items;

internal class EmemyItemPanelMenu
{
    public EmemyItemPanelMenu(Menu overlayMenu)
    {
        var ememyItemsPanelMenu = overlayMenu.CreateMenu("Ememy Items Panel");
        EmemyItemPanelItem = ememyItemsPanelMenu.CreateSwitcher("Enable", false);
        SizeItem = ememyItemsPanelMenu.CreateSlider("Size:", 0, -20, 150);
        MoveItem = ememyItemsPanelMenu.CreateSwitcher("Move", false);
        PositionXItem = ememyItemsPanelMenu.CreateSlider("Position X:", (int)(HUDInfo.ScreenSize.X - 800), 0, 10000);
        PositionYItem = ememyItemsPanelMenu.CreateSlider("Position Y:", (int)(HUDInfo.ScreenSize.Y - 240), 0, 10000);
    }

    public MenuSwitcher EmemyItemPanelItem { get; }

    public MenuSlider SizeItem { get; }

    public MenuSwitcher MoveItem { get; }

    public MenuSlider PositionXItem { get; }

    public MenuSlider PositionYItem { get; }
}