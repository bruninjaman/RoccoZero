namespace BeAware.MenuManager.ShowMeMore;

using Divine.Menu.Items;

internal sealed class TrueSightVisionMenu
{
    public TrueSightVisionMenu(Menu showMeMoreMenu)
    {
        var trueSightVisionMenu = showMeMoreMenu.CreateMenu("True Sight Vision");
        EnableItem = trueSightVisionMenu.CreateSwitcher("Enable");
    }

    public MenuSwitcher EnableItem { get; }
}