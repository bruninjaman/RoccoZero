using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal class AncientApparitionIceBlastMenu
    {
        public AncientApparitionIceBlastMenu(Menu moreInformationMenu)
        {
            var ancientApparitionIceBlastMenu = moreInformationMenu.CreateMenu("Ancient Apparition Ice Blast").SetAbilityImage(AbilityId.ancient_apparition_ice_blast);
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