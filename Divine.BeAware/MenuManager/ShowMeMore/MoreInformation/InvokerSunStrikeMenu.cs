using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal class InvokerSunStrikeMenu
    {
        public InvokerSunStrikeMenu(Menu.Items.Menu moreInformationMenu)
        {
            var invokerSunStrikeMenu = moreInformationMenu.CreateMenu("Invoker Sun Strike").SetAbilityTexture(AbilityId.invoker_sun_strike);
            EnableItem = invokerSunStrikeMenu.CreateSwitcher("Enable");
            RedItem = invokerSunStrikeMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = invokerSunStrikeMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = invokerSunStrikeMenu.CreateSlider("Blue:", 0, 0, 255);
            SideMessageItem = invokerSunStrikeMenu.CreateSwitcher("Side Message");
            SoundItem = invokerSunStrikeMenu.CreateSwitcher("Play Sound");
            OnMinimapItem = invokerSunStrikeMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = invokerSunStrikeMenu.CreateSwitcher("Draw On World");
            WriteOnChatItem = invokerSunStrikeMenu.CreateSwitcher("Write On Chat", false);
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