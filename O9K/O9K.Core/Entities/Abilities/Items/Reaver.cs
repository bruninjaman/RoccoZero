namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_reaver)]
public class Reaver : PassiveAbility
{
    public Reaver(Ability baseAbility)
        : base(baseAbility)
    {
    }
}