using Divine.Entity.Entities.Abilities.Components;
using Divine.Input;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

using static TinkerEW.Data.Menu;

namespace TinkerEW;

internal sealed class Menu
{
    public readonly MenuSwitcher EnableScript;
    public readonly MenuHoldKey ComboHoldKey;
    public readonly MenuSelector ComboTargetSelectorMode;
    public readonly MenuSlider ComboTargetSelectorRadius;
    public readonly MenuItemToggler ComboItemsToggler;
    public readonly MenuAbilityToggler ComboAbilitiesToggler;
    public readonly MenuSwitcher ComboDoubleShiva;
    public readonly MenuSelector ComboBlinkMode;
    public readonly MenuSelector ComboLinkenBreakerMode;
    private readonly MenuSwitcher MiscDamageCalculator;
    private readonly MenuSwitcher MiscRocketsFailSwitch;
    private readonly MenuSwitcher MiscRearmFailSwitch;

    public Menu()
    {
        var RootMenu = MenuManager.CreateRootMenu("TinkerEW");
        EnableScript = RootMenu.CreateSwitcher("Enable");

        var ComboRootMenu = RootMenu.CreateMenu("Combo").SetAbilityImage(AbilityId.tinker_laser);
        ComboHoldKey = ComboRootMenu.CreateHoldKey("Key", Key.None);
        ComboTargetSelectorMode = ComboRootMenu.CreateSelector("Target Selector Mode", TargetSelectorModes);
        ComboTargetSelectorRadius = ComboRootMenu.CreateSlider("Radius", 200, 100, 1000);
        ComboItemsToggler = ComboRootMenu.CreateItemToggler("Items", ComboItems, false, true);
        ComboAbilitiesToggler = ComboRootMenu.CreateAbilityToggler("Abilities", ComboAbilities, false);
        ComboDoubleShiva = ComboRootMenu.CreateSwitcher("Double Shiva").SetAbilityImage(AbilityId.item_shivas_guard);
        ComboBlinkMode = ComboRootMenu.CreateSelector("Blink Mode", ComboBlinkModes).SetAbilityImage(AbilityId.item_blink);
        ComboLinkenBreakerMode = ComboRootMenu.CreateSelector("Linken`s Breaker Mode", LinkenBreakerModes).SetAbilityImage(AbilityId.item_sphere);

        var MiscRootMenu = RootMenu.CreateMenu("Misc");
        MiscDamageCalculator = MiscRootMenu.CreateSwitcher("Damage Calculator");
        MiscRocketsFailSwitch = MiscRootMenu.CreateSwitcher("Rockets FailSwitch").SetAbilityImage(AbilityId.tinker_heat_seeking_missile);
        MiscRearmFailSwitch = MiscRootMenu.CreateSwitcher("Rearm FailSwitch").SetAbilityImage(AbilityId.tinker_rearm);

        //TODO FARM
        //var FarmRootMenu = RootMenu.CreateMenu("Farm").SetAbilityImage(AbilityId.alchemist_goblins_greed);

        ComboTargetSelectorMode.ValueChanged += ComboTargetSelectorMode_ValueChanged;

    }

    private void ComboTargetSelectorMode_ValueChanged(MenuSelector selector, SelectorEventArgs e)
    {
        if (e.NewValue == "Nearest To Cursor")
        {
            ComboTargetSelectorRadius.IsHidden = true;
        }
        else
        {
            ComboTargetSelectorRadius.IsHidden = false;
        }
    }

    internal void Dispose()
    {
        ComboTargetSelectorMode.ValueChanged -= ComboTargetSelectorMode_ValueChanged;
    }
}