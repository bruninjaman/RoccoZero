namespace O9K.Hud
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Core.Logger;

    using Divine;

    using Helpers;

    using MainMenu;

    using Modules;

    using O9K.Hud.Helpers.Notificator;

    //[ExportPlugin("O9K // Hud", priority: int.MaxValue)]
    internal sealed class Bootstrap : Bootstrapper
    {
        private readonly List<IHudModule> modules = new List<IHudModule>();

        protected override void OnActivate()
        {
            var hudMenu = new HudMenu();
            var minimap = new Minimap(hudMenu);
            var topPanel = new TopPanel(hudMenu);
            var notificator = new Notificator(minimap, hudMenu);

            modules.Add(hudMenu);
            modules.Add(minimap);
            modules.Add(topPanel);
            modules.Add(notificator);

            var mainModules = new Dictionary<Type, IHudModule>
            {
                { typeof(IHudMenu), hudMenu },
                { typeof(IMinimap), minimap },
                { typeof(ITopPanel), topPanel },
                { typeof(INotificator), notificator }
            };

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsClass || !typeof(IHudModule).IsAssignableFrom(type))
                {
                    continue;
                }

                if (type == typeof(HudMenu) || type == typeof(Minimap) || type == typeof(TopPanel) || type == typeof(Notificator))
                {
                    continue;
                }

                var constructor = type.GetConstructors()[0];

                var parameters = constructor.GetParameters();
                var objectParameters = new object[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    objectParameters[i] = mainModules[parameter.ParameterType];
                }

                modules.Add((IHudModule)Activator.CreateInstance(type, objectParameters));
            }

            foreach (var hudModule in this.modules.OrderByDescending(x => x is IHudMenu).ThenByDescending(x => x is IMinimap))
            {
                try
                {
                    hudModule.Activate();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        protected override void OnDeactivate()
        {
            /*foreach (var hudModule in this.modules)
            {
                try
                {
                    hudModule.Dispose();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }*/
        }
    }
}
