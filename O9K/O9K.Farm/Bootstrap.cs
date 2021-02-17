namespace O9K.Farm
{
    using System;
    using System.ComponentModel.Composition;

    using Core;

    using Divine;

    using Menu;

    using O9K.Core.Logger;
    using O9K.Core.Managers.Context;

    //[ExportPlugin("O9K // Farm", priority: int.MaxValue)]
    public class Bootstrap : Bootstrapper
    {
        private FarmManager farmManager;

        private MenuManager menuManager;

        protected override void OnActivate()
        {
            try
            {
                this.menuManager = new MenuManager();
                this.farmManager = new FarmManager(this.menuManager);
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
                this.farmManager.Dispose();
                this.menuManager.Dispose();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}