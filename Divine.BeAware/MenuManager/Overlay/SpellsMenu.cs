using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.Overlay
{
    internal sealed class SpellsMenu
    {
        public SpellsMenu(Menu.Items.Menu overlayMenu)
        {
            var spellsMenu = overlayMenu.CreateMenu("Spells");
            AllyOverlayItem = spellsMenu.CreateSwitcher("Ally", false);
            EnemyOverlayItem = spellsMenu.CreateSwitcher("Enemy");
            ModeItem = spellsMenu.CreateSelector("Mode:", new[] { "Default", "Without Texture", "Low" });
            ExtraSizeItem = spellsMenu.CreateSlider("Extra Size:", 0, -10, 10);
            ExtraPosXItem = spellsMenu.CreateSlider("Extra Pos X:", 0, -150, 150);
            ExtraPosYItem = spellsMenu.CreateSlider("Extra Pos Y:", 0, -150, 150);
        }

        public MenuSwitcher AllyOverlayItem { get; }

        public MenuSwitcher EnemyOverlayItem { get; }

        public MenuSelector ModeItem { get; }

        public MenuSlider ExtraSizeItem { get; }

        public MenuSlider ExtraPosXItem { get; }

        public MenuSlider ExtraPosYItem { get; }
    }
}