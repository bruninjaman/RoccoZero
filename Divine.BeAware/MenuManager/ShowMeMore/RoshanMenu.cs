using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore
{
    internal sealed class RoshanMenu
    {
        public RoshanMenu(Menu.Items.Menu showMeMoreMenu)
        {
            var roshanMenu = showMeMoreMenu.CreateMenu("Roshan").SetTexture(@"horizontal_heroes\npc_dota_hero_roshan.png");
            PanelItem = roshanMenu.CreateSwitcher("Panel");
            AegisItem = roshanMenu.CreateSwitcher("Aegis").SetTexture(@"items\item_aegis.png");
            SideMessageItem = roshanMenu.CreateSwitcher("Side Message");
            PlaySoundItem = roshanMenu.CreateSwitcher("Play Sound");
        }

        public MenuSwitcher PanelItem { get; }

        public MenuSwitcher AegisItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher PlaySoundItem { get; }
    }
}