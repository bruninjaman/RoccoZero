namespace Debugger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Debugger.Logger;
using Debugger.Menus;

using Divine.Service;

using Tools;

//[ExportPlugin("Debugger", StartupMode.Auto, priority: 1)]
internal class Bootstrap : Bootstrapper
{
    private readonly List<IDebuggerTool> tools = new();

    private MainMenu MainMenu;

    private Log Log;

    protected override void OnMainActivate()
    {
        //UpdateManager.BeginInvoke(OnActivate);
    }

    protected override void OnActivate()
    {
        MainMenu = new MainMenu();
        MainMenu.Activate();

        Log = new Log(MainMenu);
        Log.Activate();

        var mainModules = new Dictionary<Type, object>
        {
            { typeof(IMainMenu), MainMenu },
            { typeof(ILog), Log }
        };

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!type.IsClass || !typeof(IDebuggerTool).IsAssignableFrom(type))
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

            tools.Add((IDebuggerTool)Activator.CreateInstance(type, objectParameters));
        }

        foreach (var service in this.tools.OrderByDescending(x => x.LoadPriority))
        {
            service.Activate();
        }
    }

    protected override void OnDeactivate()
    {
        foreach (var service in this.tools.OrderBy(x => x.LoadPriority))
        {
            service.Dispose();
        }
    }
}