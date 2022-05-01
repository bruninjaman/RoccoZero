using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseItemNoTargetAbility : BaseItemAbility, INoTargetAbility
{
    protected BaseItemNoTargetAbility(Ability baseAbility) : base(baseAbility)
    {
    }

    protected BaseItemNoTargetAbility(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public override bool CanBeCasted()
    {
        return base.CanBeCasted();
    }

    public override bool Cast()
    {
        return CanBeCasted() && BaseAbility.Cast();
    }
}