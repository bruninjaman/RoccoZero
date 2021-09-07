namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_ring_of_health)]
public class RingOfHealth : PassiveAbility
{
    public RingOfHealth(Ability baseAbility)
        : base(baseAbility)
    {
    }
}