namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_mango_tree)]
public class MangoTree : RangedAbility
{
    public MangoTree(Ability baseAbility)
        : base(baseAbility)
    {
    }
}