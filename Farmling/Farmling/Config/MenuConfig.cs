using Divine.Menu;
using Divine.Menu.Items;

namespace Farmling.Config;

public sealed class MenuConfig
{
    public readonly RootMenu RootMenu;

    public MenuConfig()
    {
        RootMenu = MenuManager
            .CreateRootMenu("Farmling");

        Enabled = RootMenu.CreateSwitcher("Enabled");
        FarmKey = RootMenu.CreateHoldKey("Farm");
        MoveToMouse = RootMenu.CreateSwitcher("Move to mouse");
        FarmCreeps = RootMenu.CreateSwitcher("Farm enemy creeps");
        DenyCreeps = RootMenu.CreateSwitcher("Deny ally creeps");
        Debugger = RootMenu.CreateSwitcher("Debugger", false);
    }

    public MenuSwitcher DenyCreeps { get; set; }

    public MenuSwitcher FarmCreeps { get; set; }

    public MenuSwitcher Enabled { get; set; }

    public MenuSwitcher MoveToMouse { get; set; }

    public MenuSwitcher Debugger { get; set; }

    public MenuHoldKey FarmKey { get; set; }
}
