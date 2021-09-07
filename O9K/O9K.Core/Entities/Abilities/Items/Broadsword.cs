namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_broadsword)]
public class Broadsword : PassiveAbility
{
    public Broadsword(Ability baseAbility)
        : base(baseAbility)
    {
    }
}