using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.Overlay
{
    internal class HpBarMenu
    {
        public HpBarMenu(Menu.Items.Menu overlayMenu)
        {
            var hpBarMenu = overlayMenu.CreateMenu("Hp Bar");
            HpBarValueItem = hpBarMenu.CreateSwitcher("Value", false);
        }

        public MenuSwitcher HpBarValueItem { get; }
    }
}