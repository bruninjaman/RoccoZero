using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore
{
    public class CheckRuneMenu
    {
        public CheckRuneMenu(Menu showMeMoreMenu)
        {
            var checkRuneMenu = showMeMoreMenu.CreateMenu("Check Rune").SetImage("rune_regen", MenuImageType.Ability);
            EnableItem = checkRuneMenu.CreateSwitcher("Enable");
            SideMessageItem = checkRuneMenu.CreateSwitcher("Side Message");
            PlaySoundItem = checkRuneMenu.CreateSwitcher("Play Sound");
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher PlaySoundItem { get; }
    }
}