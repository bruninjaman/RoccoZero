namespace O9K.Evader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Logger;

    using Divine;

    using Metadata;

    //[ExportPlugin("O9K // Evader", priority: int.MaxValue)]
    internal class Bootstrap : Bootstrapper
    {
        private readonly IEnumerable<IEvaderService> evaderServices;

        public Bootstrap([ImportMany] IEnumerable<IEvaderService> services)
        {
            this.evaderServices = services;
        }

        protected override void OnActivate()
        {
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