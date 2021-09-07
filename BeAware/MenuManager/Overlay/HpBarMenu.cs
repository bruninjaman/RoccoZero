namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal class HpBarMenu
{
    public HpBarMenu(Menu overlayMenu)
    {
        var hpBarMenu = overlayMenu.CreateMenu("Hp Bar");
        HpBarValueItem = hpBarMenu.CreateSwitcher("Value", false);
    }

    public MenuSwitcher HpBarValueItem { get; }
}