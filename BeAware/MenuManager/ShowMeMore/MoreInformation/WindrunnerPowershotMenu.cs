using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal sealed class WindrunnerPowershotMenu
    {
        public WindrunnerPowershotMenu(Menu moreInformationMenu)
        {
            var windrunnerPowershotMenu = moreInformationMenu.CreateMenu("Windrunner Powershot").SetAbilityImage(AbilityId.windrunner_powershot);
            EnableItem = windrunnerPowershotMenu.CreateSwitcher("Enable");
            RedItem = windrunnerPowershotMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = windrunnerPowershotMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = windrunnerPowershotMenu.CreateSlider("Blue:", 0, 0, 255);
            WhenIsVisibleItem = windrunnerPowershotMenu.CreateSwitcher("When Is Visible", false);
            SideMessageItem = windrunnerPowershotMenu.CreateSwitcher("Side Message", false);
            SoundItem = windrunnerPowershotMenu.CreateSwitcher("Play Sound", false);
            OnMinimapItem = windrunnerPowershotMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = windrunnerPowershotMenu.CreateSwitcher("Draw On World");
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSwitcher WhenIsVisibleItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher SoundItem { get; }

        public MenuSwitcher OnMinimapItem { get; }

        public MenuSwitcher OnWorldItem { get; }
    }
}