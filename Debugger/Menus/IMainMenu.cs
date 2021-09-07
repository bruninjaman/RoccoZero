namespace Debugger.Menus;

using Divine.Menu.Items;

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