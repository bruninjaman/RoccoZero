using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Numerics;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseItemPointAbility : BaseItemAbility, IPointAbility
{
    protected BaseItemPointAbility(Ability baseAbility) : base(baseAbility)
    {
    }

    protected BaseItemPointAbility(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public override bool Cast(Vector3 targetPosition, Unit target)
    {
        return CanBeCasted(targetPosition) && BaseAbility.Cast(targetPosition);
    }

    // public abstract bool CanBeCasted(Vector3 targetPosition);
}