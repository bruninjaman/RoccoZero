namespace O9K.Core.Plugins.TestPlugin
{
    using Divine;

    using O9K.Core.Managers.Context;
    using O9K.Core.Managers.Menu.Items;

    internal sealed class TestPlugin : Bootstrapper
    {
        protected override void OnActivate()
        {
            var menuManager = Context9.MenuManager;
            //context.Renderer.TextureManager.LoadFromDota("o9k.block", @"panorama\images\hud\reborn\ping_icon_retreat_psd.vtex_c");

            var menu = new Menu("TestPlugin");
            menuManager.AddRootMenu(menu);
        }

        protected override void OnDeactivate()
        {
        }
    }
}