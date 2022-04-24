using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using InvokerAnnihilation.Constants;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseInvokablePointAbstractAbility : BaseInvokableAbstractAbility, IPointAbility
{
    protected BaseInvokablePointAbstractAbility(Ability baseAbility, AbilityId[] spheres) : base(baseAbility, spheres)
    {
    }


    public override bool Cast(Vector3 targetPosition, Unit target)
    {
        if (IsValid && CanBeCasted(targetPosition))
        {
            if (!IsInvoked)
            {
                if (CanBeInvoked())
                {
                    var invoked = Invoke();
                    if (!invoked)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return !BaseAbility!.Cast(targetPosition);
        }

        return false;
    }

    public override bool ChainCast(Unit? target, Vector3 forcedTargetPosition = default, bool checkForStun = true,
        bool checkForInvul = true)
    {
        if (target == null)
            return false;
        var targetPosition = forcedTargetPosition.IsZero ? target.Position : forcedTargetPosition;

        var isInvul = target.TargetIsInvul(out var remainingInvul);
        var isStunned = target.IsStunned(out var remainingStun);
        var hitTime = GetHitTime(targetPosition);
        var canCastForStun = false;
        var canCastForInvul = false;
        if (isStunned)
        {
            canCastForStun = remainingStun >= hitTime;
            if (checkForStun && canCastForStun)
            {
                return !BaseAbility!.Cast(targetPosition);
            }
            else
            {
            }
        }

        if (isInvul)
        {
            canCastForInvul = remainingInvul <= hitTime;
            if (checkForInvul && canCastForInvul)
            {
                return !BaseAbility!.Cast(targetPosition);
            }
            else
            {
            }
        }

        return true;
    }

    public virtual float CastRange => BaseAbility!.GetCastRange();

    public override bool CanBeCasted(Vector3 targetPosition)
    {
        if (base.CanBeCasted() && Owner.Position.IsInRange(targetPosition, CastRange))
        {
            return true;
        }

        return false;
    }

    public override bool CanBeCasted(Unit? target)
    {
        if (target == null)
            return false;
        return (!target.IsMagicImmune() || UseOnMagicImmuneTarget);

        return false;
    }
}