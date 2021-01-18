using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore
{
    public class CheckHandOfMidasMenu
    {
        public CheckHandOfMidasMenu(Menu.Items.Menu showMeMoreMenu)
        {
            var checkHandOfMidasMenu = showMeMoreMenu.CreateMenu("Check Hand Of Midas").SetAbilityTexture(AbilityId.item_hand_of_midas, MenuAbilityTextureType.Item);
            EnableItem = checkHandOfMidasMenu.CreateSwitcher("Enable");
            SideMessageItem = checkHandOfMidasMenu.CreateSwitcher("Side Message");
            PlaySoundItem = checkHandOfMidasMenu.CreateSwitcher("Play Sound");
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher PlaySoundItem { get; }
    }
}