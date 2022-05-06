using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities.Items;

[Ability(AbilityId.item_cyclone, AbilityId.item_wind_waker)]
public class Eul : BaseItemTargetAbility
{
    public Eul(Ability baseAbility) : base(baseAbility)
    {
    }

    public Eul(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public override bool HitEnemy { get; } = true;
}