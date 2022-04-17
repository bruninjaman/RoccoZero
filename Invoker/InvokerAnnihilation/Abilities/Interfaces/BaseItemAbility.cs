using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseItemAbility : BaseAbstractAbility
{
    protected BaseItemAbility(Ability baseAbility) : base(baseAbility)
    {
    }

    protected BaseItemAbility(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public virtual float CastRange => BaseAbility?.GetCastRange() ?? 0;
}