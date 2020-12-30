using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.Overlay
{
    internal class TopPanelMenu
    {
        public TopPanelMenu(Menu.Items.Menu overlayMenu)
        {
            var topPanelMenu = overlayMenu.CreateMenu("Top Panel");

            VisibleStatusMenu = new VisibleStatusMenu(topPanelMenu);
            UltimateBarMenu = new UltimateBarMenu(topPanelMenu);

            HealthManaBarItem = topPanelMenu.CreateSwitcher("Health and Mana Bar");
        }

        public VisibleStatusMenu VisibleStatusMenu { get; }

        public UltimateBarMenu UltimateBarMenu { get; }

        public MenuSwitcher HealthManaBarItem { get; }
    }
}