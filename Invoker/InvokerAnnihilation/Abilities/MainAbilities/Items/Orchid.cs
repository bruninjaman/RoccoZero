using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities.Items;

[Ability(AbilityId.item_orchid, AbilityId.item_bloodthorn)]
public class Orchid : BaseItemTargetAbility
{
    public Orchid(Ability baseAbility) : base(baseAbility)
    {
    }

    public Orchid(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }

    public override bool HitEnemy { get; } = true;
}