namespace BeAware.MenuManager.ShowMeMore;

using Divine.Menu.Items;

internal sealed class LinkenShowMenu
{
    public LinkenShowMenu(Menu showMeMoreMenu)
    {
        var linkenShowMenu = showMeMoreMenu.CreateMenu("Linken Show");
        EnableItem = linkenShowMenu.CreateSwitcher("Enable");
    }

    public MenuSwitcher EnableItem { get; }
}