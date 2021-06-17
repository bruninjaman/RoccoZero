namespace O9K.Farm
{
    using System;

    using Core;

    using Divine.Service;

    using Menu;

    using O9K.Core.Logger;

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