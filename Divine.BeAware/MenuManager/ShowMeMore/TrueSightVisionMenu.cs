using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore
{
    internal sealed class TrueSightVisionMenu
    {
        public TrueSightVisionMenu(Menu.Items.Menu showMeMoreMenu)
        {
            var trueSightVisionMenu = showMeMoreMenu.CreateMenu("True Sight Vision");
            EnableItem = trueSightVisionMenu.CreateSwitcher("Enable");
        }

        public MenuSwitcher EnableItem { get; }
    }
}