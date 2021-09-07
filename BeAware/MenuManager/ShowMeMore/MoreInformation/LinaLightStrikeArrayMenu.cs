namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

internal sealed class LinaLightStrikeArrayMenu
{
    public LinaLightStrikeArrayMenu(Menu moreInformationMenu)
    {
        var linaLightStrikeArrayMenu = moreInformationMenu.CreateMenu("Lina Light Strike Array").SetAbilityImage(AbilityId.lina_light_strike_array);
        EnableItem = linaLightStrikeArrayMenu.CreateSwitcher("Enable");
        RedItem = linaLightStrikeArrayMenu.CreateSlider("Red:", 255, 0, 255);
        GreenItem = linaLightStrikeArrayMenu.CreateSlider("Green:", 0, 0, 255);
        BlueItem = linaLightStrikeArrayMenu.CreateSlider("Blue:", 0, 0, 255);
        WhenIsVisibleItem = linaLightStrikeArrayMenu.CreateSwitcher("When Is Visible", false);
        SideMessageItem = linaLightStrikeArrayMenu.CreateSwitcher("Side Message", false);
        SoundItem = linaLightStrikeArrayMenu.CreateSwitcher("Play Sound", false);
        OnMinimapItem = linaLightStrikeArrayMenu.CreateSwitcher("Draw On Minimap");
        OnWorldItem = linaLightStrikeArrayMenu.CreateSwitcher("Draw On World");
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