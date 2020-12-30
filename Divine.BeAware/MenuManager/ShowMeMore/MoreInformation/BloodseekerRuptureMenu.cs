using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal sealed class BloodseekerRuptureMenu
    {
        public BloodseekerRuptureMenu(Menu.Items.Menu moreInformationMenu)
        {
            var bloodseekerRuptureMenu = moreInformationMenu.CreateMenu("Bloodseeker Rupture").SetTexture(@"spells\bloodseeker_rupture.png");
            EnableItem = bloodseekerRuptureMenu.CreateSwitcher("Enable");
            AutoStopItem = bloodseekerRuptureMenu.CreateSwitcher("Auto Stop");
            RedItem = bloodseekerRuptureMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = bloodseekerRuptureMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = bloodseekerRuptureMenu.CreateSlider("Blue:", 0, 0, 255);
            AlphaItem = bloodseekerRuptureMenu.CreateSlider("Alpha:", 40, 0, 255);
        }

        public MenuSwitcher EnableItem { get; }

        public MenuSwitcher AutoStopItem { get; }

        public MenuSlider RedItem { get; }

        public MenuSlider GreenItem { get; }

        public MenuSlider BlueItem { get; }

        public MenuSlider AlphaItem { get; }
    }
}