namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_gem)]
public class GemOfTrueSight : RangedAbility
{
    public GemOfTrueSight(Ability baseAbility)
        : base(baseAbility)
    {
    }
}