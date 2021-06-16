using Divine.Menu.Items;

namespace BeAware.MenuManager.Overlay
{
    internal class UltimateBarMenu
    {
        public UltimateBarMenu(Menu topPanelMenu)
        {
            var ultimateBarMenu = topPanelMenu.CreateMenu("Ultimate Bar");

            UltimateBarAllyItem = ultimateBarMenu.CreateSwitcher("Ally");
            UltimateBarEnemyItem = ultimateBarMenu.CreateSwitcher("Enemy");
        }

        public MenuSwitcher UltimateBarAllyItem { get; set; }

        public MenuSwitcher UltimateBarEnemyItem { get; set; }
    }
}