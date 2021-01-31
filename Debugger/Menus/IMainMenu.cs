using Divine.Menu.Items;

namespace Debugger.Menus
{
    internal interface IMainMenu
    {
        Menu CheatsMenu { get; }

        Menu GameEventsMenu { get; }

        Menu InformationMenu { get; }

        Menu OnAddRemoveMenu { get; }

        Menu OnChangeMenu { get; }

        Menu OnExecuteOrderMenu { get; }

        Menu OverlaySettingsMenu { get; }
    }
}