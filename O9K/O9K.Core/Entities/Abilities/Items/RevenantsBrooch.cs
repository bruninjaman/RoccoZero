namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_revenants_brooch)]
public class RevenantsBrooch : ActiveAbility
{
    public RevenantsBrooch(Ability baseAbility)
        : base(baseAbility)
    {
    }
}