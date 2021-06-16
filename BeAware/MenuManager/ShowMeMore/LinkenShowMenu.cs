using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore
{
    internal sealed class LinkenShowMenu
    {
        public LinkenShowMenu(Menu showMeMoreMenu)
        {
            var linkenShowMenu = showMeMoreMenu.CreateMenu("Linken Show");
            EnableItem = linkenShowMenu.CreateSwitcher("Enable");
        }

        public MenuSwitcher EnableItem { get; }
    }
}