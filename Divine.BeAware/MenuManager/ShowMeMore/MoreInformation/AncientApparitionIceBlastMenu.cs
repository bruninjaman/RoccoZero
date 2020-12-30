using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal class AncientApparitionIceBlastMenu
    {
        public AncientApparitionIceBlastMenu(Menu.Items.Menu moreInformationMenu)
        {
            var ancientApparitionIceBlastMenu = moreInformationMenu.CreateMenu("Ancient Apparition Ice Blast").SetTexture(@"spells\ancient_apparition_ice_blast.png");
            EnableItem = ancientApparitionIceBlastMenu.CreateSwitcher("Enable");
            RedItem = ancientApparitionIceBlastMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = ancientApparitionIceBlastMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = ancientApparitionIceBlastMenu.CreateSlider("Blue:", 0, 0, 255);
            SideMessageItem = ancientApparitionIceBlastMenu.CreateSwitcher("Side Message");
            SoundItem = ancientApparitionIceBlastMenu.CreateSwitcher("Play Sound");
            OnMinimapItem = ancientApparitionIceBlastMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = ancientApparitionIceBlastMenu.CreateSwitcher("Draw On World");
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