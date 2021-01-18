using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore
{
    public class CheckRuneMenu
    {
        public CheckRuneMenu(Menu.Items.Menu showMeMoreMenu)
        {
            var checkRuneMenu = showMeMoreMenu.CreateMenu("Check Rune").SetTexture("rune_regen", MenuTextureType.Ability);
            EnableItem = checkRuneMenu.CreateSwitcher("Enable");
            SideMessageItem = checkRuneMenu.CreateSwitcher("Side Message");
            PlaySoundItem = checkRuneMenu.CreateSwitcher("Play Sound");
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher PlaySoundItem { get; }
    }
}