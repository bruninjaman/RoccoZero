namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

internal sealed class BloodseekerRuptureMenu
{
    public BloodseekerRuptureMenu(Menu moreInformationMenu)
    {
        var bloodseekerRuptureMenu = moreInformationMenu.CreateMenu("Bloodseeker Rupture").SetAbilityImage(AbilityId.bloodseeker_rupture);
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