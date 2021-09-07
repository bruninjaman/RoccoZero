namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_chipped_vest)]
public class ChippedVest : PassiveAbility
{
    public ChippedVest(Ability baseAbility)
        : base(baseAbility)
    {
    }
}