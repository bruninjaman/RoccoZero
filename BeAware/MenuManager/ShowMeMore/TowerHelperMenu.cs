using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore
{
    internal sealed class TowerHelperMenu
    {
        public TowerHelperMenu(Menu showMeMoreMenu)
        {
            var towerHelperMenu = showMeMoreMenu.CreateMenu("Tower Helper");
            AutoAttackRangeItem = towerHelperMenu.CreateSwitcher("Auto Attack Range");
        }

        public MenuSwitcher AutoAttackRangeItem { get; }
    }
}