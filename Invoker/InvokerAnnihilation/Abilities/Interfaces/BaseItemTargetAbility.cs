using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseItemTargetAbility : BaseItemAbility, ITargetAbility
{
    protected BaseItemTargetAbility(Ability baseAbility) : base(baseAbility)
    {
    }

    protected BaseItemTargetAbility(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public override bool Cast(Unit target)
    {
        if (CanBeCasted(target))
        {
            return BaseAbility.Cast(target);
        }

        return false;
    }

    public override bool CanBeCasted(Unit? target)
    {
        if (base.CanBeCasted() && target != null && target.IsValidTarget(CastRange, false))
        {
            return true;
        }

        return false;
    }

    public virtual bool HitEnemy { get; } = false;
    public virtual bool HitAlly { get; } = false;
}