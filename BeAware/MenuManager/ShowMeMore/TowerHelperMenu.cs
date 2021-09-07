namespace BeAware.MenuManager.ShowMeMore;

using Divine.Menu.Items;

internal sealed class TowerHelperMenu
{
    public TowerHelperMenu(Menu showMeMoreMenu)
    {
        var towerHelperMenu = showMeMoreMenu.CreateMenu("Tower Helper");
        AutoAttackRangeItem = towerHelperMenu.CreateSwitcher("Auto Attack Range");
    }

    public MenuSwitcher AutoAttackRangeItem { get; }
}