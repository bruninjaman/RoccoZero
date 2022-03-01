namespace O9K.Farm;

using System;

using Core;

using Divine.Renderer;
using Divine.Service;

using O9K.Core.Logger;
using O9K.Core.Managers.Context;
using O9K.Farm.Menu;

using MenuCore = O9K.Core.Managers.Menu.Items.Menu;

//[ExportPlugin("O9K // Farm", priority: int.MaxValue)]
public class Bootstrap : Bootstrapper
{
    private FarmManager farmManager;

    private MenuManager menuManager;

    private MenuCore menu;

    protected override void OnMainActivate()
    {
        RendererManager.LoadImage("o9k.icon_gold", @"panorama\images\hud\icon_gold_psd.vtex_c");

        this.menu = new MenuCore("Farm", "O9K.Farm").SetTexture("o9k.icon_gold");

        Context9.MenuManager.AddRootMenu(this.menu);
    }

    protected override void OnActivate()
    {
        try
        {
            this.menuManager = new MenuManager(menu);
            this.farmManager = new FarmManager(this.menuManager);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    protected override void OnMainDeactivate()
    {
        Context9.MenuManager.RemoveRootMenu(this.menu);
    }

    protected override void OnDeactivate()
    {
        try
        {
            this.farmManager.Dispose();
            this.menuManager.Dispose();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}