using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal sealed class LeshracSplitEarthMenu
    {
        public LeshracSplitEarthMenu(Menu moreInformationMenu)
        {
            var leshracSplitEarthMenu = moreInformationMenu.CreateMenu("Leshrac Split Earth").SetAbilityImage(AbilityId.leshrac_split_earth);
            EnableItem = leshracSplitEarthMenu.CreateSwitcher("Enable");
            RedItem = leshracSplitEarthMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = leshracSplitEarthMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = leshracSplitEarthMenu.CreateSlider("Blue:", 0, 0, 255);
            WhenIsVisibleItem = leshracSplitEarthMenu.CreateSwitcher("When Is Visible", false);
            SideMessageItem = leshracSplitEarthMenu.CreateSwitcher("Side Message", false);
            SoundItem = leshracSplitEarthMenu.CreateSwitcher("Play Sound", false);
            OnMinimapItem = leshracSplitEarthMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = leshracSplitEarthMenu.CreateSwitcher("Draw On World");
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