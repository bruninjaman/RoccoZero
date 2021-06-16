using Divine.Menu.Items;

namespace BeAware.MenuManager.Overlay
{
    internal class ManaBarMenu
    {
        public ManaBarMenu(Menu overlayMenu)
        {
            var manaBarMenu = overlayMenu.CreateMenu("Mana Bar");
            ManaBarItem = manaBarMenu.CreateSwitcher("Enable");
            ManaBarValueItem = manaBarMenu.CreateSwitcher("Value", false);
        }

        public MenuSwitcher ManaBarItem { get; }

        public MenuSwitcher ManaBarValueItem { get; }
    }
}