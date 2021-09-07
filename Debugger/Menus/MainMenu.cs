namespace Debugger.Menus;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Numerics;

internal class MainMenu : IMainMenu
{
    private Menu factory;

    public Menu CheatsMenu { get; private set; }

    public Menu GameEventsMenu { get; private set; }

    public Menu InformationMenu { get; private set; }

    public int LoadPriority { get; } = 1000;

    public Menu OnAddRemoveMenu { get; private set; }

    public Menu OnChangeMenu { get; private set; }

    public Menu OnExecuteOrderMenu { get; private set; }

    public Menu OverlaySettingsMenu { get; private set; }

    public void Activate()
    {
        this.factory = MenuManager.CreateRootMenu("Debugger").SetAbilityImage(AbilityId.chaos_knight_reality_rift);
        this.factory.SetFontColor(Color.PaleVioletRed);

        this.OnAddRemoveMenu = this.factory.CreateMenu("On add/remove");
        this.OnChangeMenu = this.factory.CreateMenu("On change");
        this.InformationMenu = this.factory.CreateMenu("Information");
        this.CheatsMenu = this.factory.CreateMenu("Cheats");
        this.OnExecuteOrderMenu = this.factory.CreateMenu("On execute order");
        this.GameEventsMenu = this.factory.CreateMenu("Game events");
        this.OverlaySettingsMenu = this.factory.CreateMenu("Overlay settings");
    }

    public void Dispose() //TODO
    {
        //this.factory?.Dispose();
    }
}