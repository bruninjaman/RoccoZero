using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal class MiranaArrowMenu
    {
        public MiranaArrowMenu(Menu.Items.Menu moreInformationMenu)
        {
            var miranaArrowMenu = moreInformationMenu.CreateMenu("Mirana Arrow").SetAbilityTexture(AbilityId.mirana_arrow);
            EnableItem = miranaArrowMenu.CreateSwitcher("Enable");
            RedItem = miranaArrowMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = miranaArrowMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = miranaArrowMenu.CreateSlider("Blue:", 0, 0, 255);
            SideMessageItem = miranaArrowMenu.CreateSwitcher("Side Message");
            SoundItem = miranaArrowMenu.CreateSwitcher("Play Sound");
            OnMinimapItem = miranaArrowMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = miranaArrowMenu.CreateSwitcher("Draw On World");
            WriteOnChatItem = miranaArrowMenu.CreateSwitcher("Write On Chat", false);
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher SoundItem { get; }

        public MenuSwitcher OnMinimapItem { get; }

        public MenuSwitcher OnWorldItem { get; }

        public MenuSwitcher WriteOnChatItem { get; }
    }
}