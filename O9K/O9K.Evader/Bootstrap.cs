namespace O9K.Evader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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