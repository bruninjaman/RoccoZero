namespace O9K.Evader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Core.Logger;

    using Divine;

    using Metadata;

    using O9K.Evader.Evader.EvadeModes;
    using O9K.Evader.Settings;

    //[ExportPlugin("O9K // Evader", priority: int.MaxValue)]
    internal class Bootstrap : Bootstrapper
    {
        private readonly List<IEvaderService> evaderServices = new List<IEvaderService>();

        protected override void OnActivate()
        {
            var menuManager = new MenuManager();
            var pathfinder = new Pathfinder.Pathfinder();
            var actionManager = new ActionManager.ActionManager(menuManager);
            var abilityManager = new AbilityManager.AbilityManager(pathfinder, actionManager, menuManager);
            var evadeModeManager = new EvadeModeManager(abilityManager, pathfinder, actionManager, menuManager);
            var debugger = new Helpers.Debugger(pathfinder, menuManager, abilityManager, actionManager);

            evaderServices.Add(menuManager);
            evaderServices.Add(pathfinder);
            evaderServices.Add(actionManager);
            evaderServices.Add(abilityManager);
            evaderServices.Add(evadeModeManager);
            evaderServices.Add(debugger);

            var mainEvaderServices = new Dictionary<Type, IEvaderService>()
            {
                { typeof(IMainMenu), menuManager },
                { typeof(IPathfinder), pathfinder },
                { typeof(IActionManager), actionManager },
                { typeof(IAbilityManager), abilityManager },
                { typeof(IEvadeModeManager), evadeModeManager },
                { typeof(IDebugger), debugger }
            };

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsClass || !typeof(IEvaderService).IsAssignableFrom(type))
                {
                    continue;
                }

                if (type == typeof(MenuManager) ||
                    type == typeof(Pathfinder.Pathfinder) ||
                    type == typeof(ActionManager.ActionManager) ||
                    type == typeof(AbilityManager.AbilityManager) ||
                    type == typeof(EvadeModeManager) ||
                    type == typeof(Helpers.Debugger))
                {
                    continue;
                }

                var constructor = type.GetConstructors()[0];

                var parameters = constructor.GetParameters();
                var objectParameters = new object[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    objectParameters[i] = mainEvaderServices[parameter.ParameterType];
                }

                evaderServices.Add((IEvaderService)Activator.CreateInstance(type, objectParameters));
            }

            try
            {
                foreach (var service in this.evaderServices.OrderBy(x => x.LoadOrder))
                {
                    service.Activate();
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
                foreach (var service in this.evaderServices.OrderByDescending(x => x.LoadOrder))
                {
                    service.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}