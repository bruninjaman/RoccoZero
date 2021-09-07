namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_witless_shako)]
public class WitlessShako : PassiveAbility
{
    public WitlessShako(Ability baseAbility)
        : base(baseAbility)
    {
    }
}