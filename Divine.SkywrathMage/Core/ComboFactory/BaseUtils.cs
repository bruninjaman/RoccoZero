using Divine.SDK.Extensions;
using Divine.SDK.Managers.Log;

namespace Divine.Core.ComboFactory
{
    public class BaseUtils
    {
        public BaseUtils()
        {
            Owner = EntityManager.LocalHero;
        }

        protected Hero Owner { get; }

        protected virtual bool IsStopped
        {
            get
            {
                return GameManager.IsPaused || !Owner.IsValid || !Owner.IsAlive || Owner.IsStunned();
            }
        }

        protected static Log Log { get; } = LogManager.GetCurrentClassLogger();

        protected virtual Unit CurrentTarget { get; set; }

        protected bool IsNullTarget(Unit target)
        {
            if (target == null)
            {
                return true;
            }

            return false;
        }
    }
}
