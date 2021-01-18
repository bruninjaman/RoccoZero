using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal class SpiritBreakerChargeMenu
    {
        public SpiritBreakerChargeMenu(Menu.Items.Menu moreInformationMenu)
        {
            var spiritBreakerChargeMenu = moreInformationMenu.CreateMenu("Spirit Breaker Charge").SetAbilityTexture(AbilityId.spirit_breaker_charge_of_darkness);
            EnableItem = spiritBreakerChargeMenu.CreateSwitcher("Enable");
            RedItem = spiritBreakerChargeMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = spiritBreakerChargeMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = spiritBreakerChargeMenu.CreateSlider("Blue:", 0, 0, 255);
            AlphaItem = spiritBreakerChargeMenu.CreateSlider("Alpha:", 40, 0, 255);
            SideMessageItem = spiritBreakerChargeMenu.CreateSwitcher("Side Message");
            SoundItem = spiritBreakerChargeMenu.CreateSwitcher("Play Sound");
            OnMinimapItem = spiritBreakerChargeMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = spiritBreakerChargeMenu.CreateSwitcher("Draw On World");
            WriteOnChatItem = spiritBreakerChargeMenu.CreateSwitcher("Write On Chat", false);
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }

        public MenuSlider AlphaItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher SoundItem { get; }

        public MenuSwitcher OnMinimapItem { get; }

        public MenuSwitcher OnWorldItem { get; }

        public MenuSwitcher WriteOnChatItem { get; }
    }
}