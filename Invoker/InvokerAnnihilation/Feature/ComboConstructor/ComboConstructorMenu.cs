using Divine.Menu.Items;
using Divine.Numerics;
using InvokerAnnihilation.Config;
using InvokerAnnihilation.Feature.MenuBase;

namespace InvokerAnnihilation.Feature.ComboConstructor;

public sealed class ComboConstructorMenu : MenuFeatureWithDrawingBase
{
    public ComboConstructorMenu(RootMenu rootMenu) : base(rootMenu, "Combo Constructor",
        new Vector3(30, 18, 50))
    {
        UseCustomBuilder = Menu.CreateSwitcher("Use custom combo builder", false);
        ComboCount = Menu.CreateSlider("Count of combos", 6, 1, 15);
        MaxAbilitiesPerCombo = Menu.CreateSlider("Abilities per combo", 6, 3, 10);
    }

    public MenuSwitcher UseCustomBuilder { get; set; }

    public MenuSlider MaxAbilitiesPerCombo { get; set; }

    public MenuSlider ComboCount { get; set; }
}