using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Extensions;
using Divine.Numerics;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseInvokablePointAbstractAbility : BaseInvokableAbstractAbility, IPointAbility
{
    protected BaseInvokablePointAbstractAbility(Ability baseAbility, AbilityId[] spheres) : base(baseAbility, spheres)
    {
    }

    public override bool Cast(Vector3 targetPosition)
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
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return BaseAbility!.Cast(targetPosition);
        }

        return false;
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
}