using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities.Items;

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