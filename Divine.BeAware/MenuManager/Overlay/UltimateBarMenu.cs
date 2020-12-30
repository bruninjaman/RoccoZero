using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.Overlay
{
    internal class UltimateBarMenu
    {
        public UltimateBarMenu(Menu.Items.Menu topPanelMenu)
        {
            var ultimateBarMenu = topPanelMenu.CreateMenu("Ultimate Bar");

            UltimateBarAllyItem = ultimateBarMenu.CreateSwitcher("Ally");
            UltimateBarEnemyItem = ultimateBarMenu.CreateSwitcher("Enemy");
        }

        public MenuSwitcher UltimateBarAllyItem { get; set; }

        public MenuSwitcher UltimateBarEnemyItem { get; set; }
    }
}