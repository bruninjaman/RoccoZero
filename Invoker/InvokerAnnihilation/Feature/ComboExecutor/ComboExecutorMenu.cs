using Divine.Menu.Items;
using InvokerAnnihilation.Config;
using InvokerAnnihilation.Feature.MenuBase;

namespace InvokerAnnihilation.Feature.ComboExecutor;

public sealed class ComboExecutorMenu : MenuFeatureWithHoldKeyBase
{
    public ComboExecutorMenu(RootMenu rootMenu) : base(rootMenu, "Combo", "Combo key")
    {
    }
}