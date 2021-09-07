namespace BeAware.MenuManager.ShowMeMore;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

public class CheckHandOfMidasMenu
{
    public CheckHandOfMidasMenu(Menu showMeMoreMenu)
    {
        var checkHandOfMidasMenu = showMeMoreMenu.CreateMenu("Check Hand Of Midas").SetAbilityImage(AbilityId.item_hand_of_midas, MenuAbilityImageType.Item);
        EnableItem = checkHandOfMidasMenu.CreateSwitcher("Enable");
        SideMessageItem = checkHandOfMidasMenu.CreateSwitcher("Side Message");
        PlaySoundItem = checkHandOfMidasMenu.CreateSwitcher("Play Sound");
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher PlaySoundItem { get; }
}