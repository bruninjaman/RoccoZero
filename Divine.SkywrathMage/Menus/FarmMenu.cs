using Divine.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class FarmMenu
    {
        public FarmMenu(Menu.Items.Menu menu)
        {
            var farmMenu = menu.CreateMenu("Farm");
            FarmHotkeyItem = farmMenu.CreateHoldKey("Farm Hotkey");
            FarmItem = farmMenu.CreateSelector("Farm:", new[] { "Arcane Bolt & Attack", "Attack" });
            HeroHarrasItem = farmMenu.CreateSelector("Hero Harras:", new[] { "Attack", "Arcane Bolt & Attack", "Disable" });
        }

        public MenuHoldKey FarmHotkeyItem { get; }

        public MenuSelector FarmItem { get; }

        public MenuSelector HeroHarrasItem { get; }
    }
}