namespace O9K.AutoUsage
{
    using System;

    using Core.Logger;

    using Divine;

    using O9K.Core.Managers.Context;

    using Settings;

    //[ExportPlugin("O9K // Auto usage", priority: int.MaxValue)]
    internal class Bootstrap : Bootstrapper
    {
        private AutoUsage autoUsage;

        private MainSettings settings;

        protected override void OnActivate()
        {
            try
            {
                RendererManager.LoadTexture("o9k.glyph", @"panorama\images\hud\reborn\icon_glyph_on_psd.vtex_c");

                this.settings = new MainSettings(Context9.MenuManager);
                this.autoUsage = new AutoUsage(this.settings);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
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
}