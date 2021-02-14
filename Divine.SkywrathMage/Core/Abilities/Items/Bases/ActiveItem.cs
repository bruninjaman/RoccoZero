using System.Linq;

using Divine.Core.Helpers;
using Divine.SDK.Extensions;
using Divine.SDK.Helpers;
using Divine.SDK.Managers.Log;

using SharpDX;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class ActiveItem : CItem, IActiveAbility
    {
        private static readonly Log Log = LogManager.GetCurrentClassLogger();

        protected ActiveItem(Item item)
            : base(item)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                if (!IsReady || !IsEnabled)
                {
                    return false;
                }


                if (Owner.IsStunned() || Owner.IsMuted())
                {
                    return false;
                }

                return true;
            }
        }

        public virtual float CastPoint
        {
            get
            {
                var level = Level;
                if (level == 0)
                {
                    return 0.0f;
                }

                return GetCastPoint(level - 1);
            }
        }

        public override float Speed { get; } = float.MaxValue;

        protected float LastCastAttempt { get; set; }

        public virtual bool CanHit(Unit target)
        {
            return Owner.Distance2D(target) < CastRange;
        }

        public virtual int GetCastDelay(Unit target)
        {
            return (int)(((CastPoint + Owner.TurnTime(target.Position)) * 1000.0f) + GameManager.Ping);
        }

        public virtual int GetCastDelay(Vector3 position)
        {
            return (int)(((CastPoint + Owner.TurnTime(position)) * 1000.0f) + GameManager.Ping);
        }

        public virtual int GetCastDelay()
        {
            return (int)((CastPoint * 1000.0f) + GameManager.Ping);
        }

        public override float GetDamage(params Unit[] targets)
        {
            var damage = RawDamage;
            if (damage == 0)
            {
                return 0;
            }

            var amplify = Owner.GetSpellAmplification();
            var reduction = 0.0f;
            if (targets.Any())
            {
                reduction = this.GetDamageReduction(targets.First(), DamageType);
            }

            return DamageHelpers.GetSpellDamage(damage, amplify, reduction);
        }

        public override float GetDamage(Unit target, float damageModifier, float targetHealth = float.MinValue)
        {
            var damage = RawDamage;
            if (damage == 0)
            {
                return 0;
            }

            var amplify = Owner.GetSpellAmplification();
            var reduction = this.GetDamageReduction(target, DamageType);

            return DamageHelpers.GetSpellDamage(damage, amplify, -reduction, damageModifier);
        }

        public virtual int GetHitTime(Unit target)
        {
            return GetHitTime(target.Position);
        }

        public virtual int GetHitTime(Vector3 position)
        {
            if (Speed == float.MaxValue || Speed == 0)
            {
                return GetCastDelay(position) + (int)(ActivationDelay * 1000.0f);
            }

            var time = Owner.Distance2D(position) / Speed;
            return GetCastDelay(position) + (int)((time + ActivationDelay) * 1000.0f);
        }

        private readonly Sleeper castSleeper = new Sleeper();

        public override bool Cast()
        {
            if (castSleeper.Sleeping)
            {
                return false;
            }

            var result = base.Cast();
            if (result)
            {
                castSleeper.Sleep(100);
            }

            return result;
        }

        public override bool Cast(Unit target)
        {
            if (castSleeper.Sleeping)
            {
                return false;
            }

            bool result;
            if ((AbilityBehavior & AbilityBehavior.UnitTarget) == AbilityBehavior.UnitTarget)
            {
                result = base.Cast(target);
            }
            else if ((AbilityBehavior & AbilityBehavior.Point) == AbilityBehavior.Point)
            {
                result = base.Cast(target.Position);
            }
            else
            {
                result = base.Cast();
            }

            if (result)
            {
                castSleeper.Sleep(100);
            }

            return result;
        }

        public override bool Cast(Tree target)
        {
            if (castSleeper.Sleeping)
            {
                return false;
            }

            var result = base.Cast(target);
            if (result)
            {
                castSleeper.Sleep(100);
            }

            return result;
        }

        public override bool Cast(Vector3 position)
        {
            if (castSleeper.Sleeping)
            {
                return false;
            }

            bool result;
            if ((AbilityBehavior & AbilityBehavior.Point) == AbilityBehavior.Point)
            {
                result = base.Cast(position);
            }
            else
            {
                result = base.Cast();
            }

            if (result)
            {
                castSleeper.Sleep(100);
            }

            return result;
        }

        public override bool Cast(Rune target)
        {
            if (castSleeper.Sleeping)
            {
                return false;
            }

            var result = base.Cast(target);
            if (result)
            {
                castSleeper.Sleep(100);
            }

            return result;
        }
    }
}