namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_blight_stone)]
public class BlightStone : PassiveAbility
{
    public BlightStone(Ability baseAbility)
        : base(baseAbility)
    {
    }
}