namespace BeAware.MenuManager.ShowMeMore;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

internal sealed class RoshanMenu
{
    public RoshanMenu(Menu showMeMoreMenu)
    {
        var roshanMenu = showMeMoreMenu.CreateMenu("Roshan").SetImage("npc_dota_hero_roshan", MenuImageType.Unit);
        PanelItem = roshanMenu.CreateSwitcher("Panel");
        AegisItem = roshanMenu.CreateSwitcher("Aegis").SetAbilityImage(AbilityId.item_aegis, MenuAbilityImageType.Item);
        SideMessageItem = roshanMenu.CreateSwitcher("Side Message");
        PlaySoundItem = roshanMenu.CreateSwitcher("Play Sound");
    }

    public MenuSwitcher PanelItem { get; }

    public MenuSwitcher AegisItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher PlaySoundItem { get; }
}