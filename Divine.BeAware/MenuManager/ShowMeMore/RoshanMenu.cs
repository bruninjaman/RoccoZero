using Divine.Menu.Items;

namespace Divine.BeAware.MenuManager.ShowMeMore
{
    internal sealed class RoshanMenu
    {
        public RoshanMenu(Menu.Items.Menu showMeMoreMenu)
        {
            var roshanMenu = showMeMoreMenu.CreateMenu("Roshan").SetTexture("npc_dota_hero_roshan", MenuTextureType.Unit);
            PanelItem = roshanMenu.CreateSwitcher("Panel");
            AegisItem = roshanMenu.CreateSwitcher("Aegis").SetAbilityTexture(AbilityId.item_aegis, MenuAbilityTextureType.Item);
            SideMessageItem = roshanMenu.CreateSwitcher("Side Message");
            PlaySoundItem = roshanMenu.CreateSwitcher("Play Sound");
        }

        public MenuSwitcher PanelItem { get; }

        public MenuSwitcher AegisItem { get; }

        public MenuSwitcher SideMessageItem { get; }

        public MenuSwitcher PlaySoundItem { get; }
    }
}