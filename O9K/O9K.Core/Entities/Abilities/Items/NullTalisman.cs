namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_null_talisman)]
public class NullTalisman : PassiveAbility
{
    public NullTalisman(Ability baseAbility)
        : base(baseAbility)
    {
    }
}