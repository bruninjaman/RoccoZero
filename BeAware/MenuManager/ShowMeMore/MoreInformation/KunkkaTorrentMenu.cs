using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal sealed class KunkkaTorrentMenu
    {
        public KunkkaTorrentMenu(Menu moreInformationMenu)
        {
            var kunkkaTorrentMenu = moreInformationMenu.CreateMenu("Kunkka Torrent").SetAbilityImage(AbilityId.kunkka_torrent);
            EnableItem = kunkkaTorrentMenu.CreateSwitcher("Enable");
            RedItem = kunkkaTorrentMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = kunkkaTorrentMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = kunkkaTorrentMenu.CreateSlider("Blue:", 0, 0, 255);
            WhenIsVisibleItem = kunkkaTorrentMenu.CreateSwitcher("When Is Visible", false);
            SideMessageItem = kunkkaTorrentMenu.CreateSwitcher("Side Message", false);
            SoundItem = kunkkaTorrentMenu.CreateSwitcher("Play Sound", false);
            OnMinimapItem = kunkkaTorrentMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = kunkkaTorrentMenu.CreateSwitcher("Draw On World");
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }

        public MenuSwitcher WhenIsVisibleItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher SoundItem { get; }

        public MenuSwitcher OnMinimapItem { get; }

        public MenuSwitcher OnWorldItem { get; }
    }
}