using Divine.Service;

namespace ShowEnemyRoles
{
    internal sealed class Bootstrap : Bootstrapper
    {
        private Context? Context;

        protected override void OnMainActivate()
        {
            Context = new Context();
        }

        protected override void OnDeactivate()
        {
            Context?.Dispose();
        }
    }
}