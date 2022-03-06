using Divine.Service;
using Divine.Update;

namespace Overwolf
{
    internal class Bootstrap : Bootstrapper
    {
        private Context context;

        protected override void OnMainActivate()
        {
            context = new Context();
        }

        protected override void OnDeactivate()
        {
            context?.Dispose();
        }
    }
}