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


        FarmKey = RootMenu.CreateHoldKey("Farm");
        MoveToMouse = RootMenu.CreateSwitcher("Move to mouse");
        Debugger = RootMenu.CreateSwitcher("Debugger");
    }

    public MenuSwitcher MoveToMouse { get; set; }

    public MenuSwitcher Debugger { get; set; }

    public MenuHoldKey FarmKey { get; set; }
}
