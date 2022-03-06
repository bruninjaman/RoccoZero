namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_bloodstone)]
public class Bloodstone : ActiveAbility
{
    public Bloodstone(Ability baseAbility)
        : base(baseAbility)
    {
    }
}