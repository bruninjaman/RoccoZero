using Divine.Menu.Items;

namespace BeAware.MenuManager.Overlay
{
    internal class ItemsMenu
    {
        public ItemsMenu(Menu overlayMenu)
        {
            var itemsMenu = overlayMenu.CreateMenu("Items");
            AllyOverlayItem = itemsMenu.CreateSwitcher("Ally", false);
            EnemyOverlayItem = itemsMenu.CreateSwitcher("Enemy", true);
            ExtraSizeItem = itemsMenu.CreateSlider("Extra Size:", 0, -10, 10);
            ExtraPosXItem = itemsMenu.CreateSlider("Extra Pos X:", 0, -150, 150);
            ExtraPosYItem = itemsMenu.CreateSlider("Extra Pos Y:", 0, -150, 150);
        }

        public MenuSwitcher AllyOverlayItem { get; }

        public MenuSwitcher EnemyOverlayItem { get; }

        public MenuSlider ExtraSizeItem { get; }

        public MenuSlider ExtraPosXItem { get; }

        public MenuSlider ExtraPosYItem { get; }
    }
}