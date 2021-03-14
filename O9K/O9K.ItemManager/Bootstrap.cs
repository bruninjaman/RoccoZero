namespace O9K.ItemManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Core.Logger;

    using Divine;

    using Metadata;

    using O9K.ItemManager.Menu;
    using O9K.ItemManager.Modules.OrderHelper;

    //[ExportPlugin("O9K // Item manager", priority: int.MaxValue)]
    internal class Bootstrap : Bootstrapper
    {
        private readonly List<IModule> modules = new List<IModule>();

        protected override void OnActivate()
        {
            try
            {
                var mainMenu = new MainMenu();
                var orderSync = new OrderSync();

                modules.Add(mainMenu);

                var mainModules = new Dictionary<Type, object>
                {
                    { typeof(IMainMenu), mainMenu },
                    { typeof(IOrderSync), orderSync }
                };

                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (!type.IsClass || !typeof(IModule).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    if (type == typeof(MainMenu))
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

                    modules.Add((IModule)Activator.CreateInstance(type, objectParameters));
                }

                foreach (var module in this.modules.OrderByDescending(x => x is IMainMenu))
                {
                    module.Activate();
                }
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
                foreach (var module in this.modules)
                {
                    module.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}