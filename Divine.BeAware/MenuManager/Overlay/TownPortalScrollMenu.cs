using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.Overlay
{
    internal sealed class TownPortalScrollMenu
    {
        public TownPortalScrollMenu(Menu.Items.Menu overlayMenu)
        {
            var townPortalScrollMenu = overlayMenu.CreateMenu("Town Portal Scroll");
            AllyItem = townPortalScrollMenu.CreateSwitcher("Ally");
            EnemyItem = townPortalScrollMenu.CreateSwitcher("Enemy");
        }

        public MenuSwitcher AllyItem { get; }

        public MenuSwitcher EnemyItem { get; }
    }
}