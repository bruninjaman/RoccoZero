using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal sealed class PudgeHookMenu
    {
        public PudgeHookMenu(Menu.Items.Menu moreInformationMenu)
        {
            var pudgeHookMenu = moreInformationMenu.CreateMenu("Pudge Hook").SetTexture(@"spells\pudge_meat_hook.png");
            EnableItem = pudgeHookMenu.CreateSwitcher("Enable");
            RedItem = pudgeHookMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = pudgeHookMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = pudgeHookMenu.CreateSlider("Blue:", 0, 0, 255);
            WhenIsVisibleItem = pudgeHookMenu.CreateSwitcher("When Is Visible", false);
            SideMessageItem = pudgeHookMenu.CreateSwitcher("Side Message", false);
            SoundItem = pudgeHookMenu.CreateSwitcher("Play Sound", false);
            OnMinimapItem = pudgeHookMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = pudgeHookMenu.CreateSwitcher("Draw On World");
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