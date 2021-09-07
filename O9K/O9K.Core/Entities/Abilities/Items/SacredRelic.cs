namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_relic)]
public class SacredRelic : PassiveAbility
{
    public SacredRelic(Ability baseAbility)
        : base(baseAbility)
    {
    }
}