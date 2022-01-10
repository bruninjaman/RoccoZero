namespace O9K.Farm.Menu;

using System;


using Divine.Renderer;

using O9K.Core.Managers.Context;
using O9K.Core.Managers.Menu;
using O9K.Core.Managers.Menu.Items;

internal class MenuManager : IDisposable
{
    public MenuManager(Menu menu)
    {
        this.UnitSettingsMenu = menu.Add(new Menu("Units", "units"));
        this.UnitSettingsMenu.AddTranslation(Lang.Ru, "Юниты");
        this.UnitSettingsMenu.AddTranslation(Lang.Cn, "单位");

        this.LastHitMenu = new LastHitMenu(menu);
        this.PushMenu = new PushMenu(menu);
        this.MarkerMenu = new MarkerMenu(menu);
    }

    public LastHitMenu LastHitMenu { get; }

    public PushMenu PushMenu { get; }

    public MarkerMenu MarkerMenu { get; }

    public Menu UnitSettingsMenu { get; }

    public void Dispose()
    {
    }
}