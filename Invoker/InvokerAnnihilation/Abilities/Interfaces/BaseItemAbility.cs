using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using InvokerAnnihilation.Abilities.MainAbilities.Items;

namespace InvokerAnnihilation.Abilities.Interfaces;

public abstract class BaseItemAbility : BaseAbstractAbility, IHasOwnerAbility
{
    protected BaseItemAbility(Ability baseAbility) : base(baseAbility)
    {
    }

    protected BaseItemAbility(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public virtual float CastRange => BaseAbility?.GetCastRange() ?? 0;
    public virtual AbilityId OwnerAbility { get; set; } = AbilityId.dota_base_ability;
}