using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Attributes;

namespace InvokerAnnihilation.Abilities.MainAbilities.Items;

[Ability(AbilityId.item_refresher, AbilityId.item_refresher_shard)]
public class Refresher : BaseItemNoTargetAbility
{
    public Refresher(Ability baseAbility) : base(baseAbility)
    {
    }

    public Refresher(Hero owner, AbilityId abilityId) : base(owner, abilityId)
    {
    }
}