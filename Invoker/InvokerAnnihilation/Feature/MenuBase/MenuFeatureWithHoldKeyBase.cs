using Divine.Menu.Items;

namespace InvokerAnnihilation.Feature.MenuBase;

public class MenuFeatureWithHoldKeyBase : IMenuFeature, IMenuFeatureWithHoldKey
{
    protected MenuFeatureWithHoldKeyBase(RootMenu rootMenu, string name, string displayName)
    {
        Menu = rootMenu.CreateMenu(name);
        Enable = Menu.CreateSwitcher("Enable");
        HoldKey = Menu.CreateHoldKey(displayName);
    }
    public Menu Menu { get; set; }
    public MenuSwitcher Enable { get; set; }
    public MenuHoldKey HoldKey { get; set; }
}