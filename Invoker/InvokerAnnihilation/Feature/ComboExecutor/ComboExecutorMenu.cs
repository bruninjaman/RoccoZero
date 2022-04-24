using Divine.Menu.Items;
using InvokerAnnihilation.Feature.MenuBase;

namespace InvokerAnnihilation.Feature.ComboExecutor;

public sealed class ComboExecutorMenu : MenuFeatureWithHoldKeyBase
{
    public ComboExecutorMenu(RootMenu rootMenu) : base(rootMenu, "Combo", "Combo key")
    {
        PrepareKey = Menu.CreateHoldKey("Prepare combo").SetTooltip("Not working with dynamic combo");
    }

    public MenuHoldKey PrepareKey { get; set; }
}