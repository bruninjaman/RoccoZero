namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_giants_ring)]
public class GiantsRing : PassiveAbility
{
    public GiantsRing(Ability baseAbility)
        : base(baseAbility)
    {
    }
}