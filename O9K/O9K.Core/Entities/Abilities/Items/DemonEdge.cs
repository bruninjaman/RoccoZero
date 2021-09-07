namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_demon_edge)]
public class DemonEdge : PassiveAbility
{
    public DemonEdge(Ability baseAbility)
        : base(baseAbility)
    {
    }
}