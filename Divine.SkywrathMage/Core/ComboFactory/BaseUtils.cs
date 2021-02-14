using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Log;
using Divine.Core.Managers.Orbwalker;
using Divine.Core.Managers.Unit;
using Divine.Core.Services;
using Divine.SDK.Extensions;
using Divine.SDK.Managers.Log;

using Ensage.SDK.Extensions;
using Ensage.SDK.Prediction;

namespace Divine.Core.ComboFactory
{
    public class BaseUtils
    {
        public BaseUtils()
        {
            Owner = EntityManager.LocalHero;
            Orbwalker = Owner.Orbwalker;
        }

        protected Hero Owner { get; }

        protected virtual OrbwalkerManager Orbwalker { get; }

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
