using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore.MoreInformation
{
    internal sealed class InvokerEMPMenu
    {
        public InvokerEMPMenu(Menu.Items.Menu moreInformationMenu)
        {
            var invokerEMPMenu = moreInformationMenu.CreateMenu("Invoker EMP").SetTexture(@"spells\invoker_emp.png");
            EnableItem = invokerEMPMenu.CreateSwitcher("Enable");
            RedItem = invokerEMPMenu.CreateSlider("Red:", 255, 0, 255);
            GreenItem = invokerEMPMenu.CreateSlider("Green:", 0, 0, 255);
            BlueItem = invokerEMPMenu.CreateSlider("Blue:", 0, 0, 255);
            WhenIsVisibleItem = invokerEMPMenu.CreateSwitcher("When Is Visible", false);
            SideMessageItem = invokerEMPMenu.CreateSwitcher("Side Message", false);
            SoundItem = invokerEMPMenu.CreateSwitcher("Play Sound", false);
            OnMinimapItem = invokerEMPMenu.CreateSwitcher("Draw On Minimap");
            OnWorldItem = invokerEMPMenu.CreateSwitcher("Draw On World");
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