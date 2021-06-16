using Divine.Menu.Items;

namespace BeAware.MenuManager.Overlay
{
    internal class HpBarMenu
    {
        public HpBarMenu(Menu overlayMenu)
        {
            var hpBarMenu = overlayMenu.CreateMenu("Hp Bar");
            HpBarValueItem = hpBarMenu.CreateSwitcher("Value", false);
        }

        public MenuSwitcher HpBarValueItem { get; }
    }
}