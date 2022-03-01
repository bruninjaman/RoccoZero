namespace O9K.AutoUsage;

using System;

using Core.Logger;

using Divine.Renderer;
using Divine.Service;

using O9K.Core.Managers.Context;

using Settings;

using MenuCore = Core.Managers.Menu.Items.Menu;

//[ExportPlugin("O9K // Auto usage", priority: int.MaxValue)]
internal class Bootstrap : Bootstrapper
{
    private MenuCore menu;

    private AutoUsage autoUsage;

    private MainSettings settings;

    protected override void OnMainActivate()
    {
        this.menu = new MenuCore("Auto usage", "O9K.AutoUsage").SetTexture("techies_focused_detonate");

        Context9.MenuManager.AddRootMenu(this.menu);
    }

    protected override void OnActivate()
    {
        try
        {
            RendererManager.LoadImage("o9k.glyph", @"panorama\images\hud\reborn\icon_glyph_on_psd.vtex_c");

            this.settings = new MainSettings(this.menu);
            this.autoUsage = new AutoUsage(this.settings);
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
            this.autoUsage.Dispose();
            this.settings.Dispose();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}