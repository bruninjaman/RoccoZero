namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_apex)]
public class Apex : PassiveAbility
{
    public Apex(Ability baseAbility)
        : base(baseAbility)
    {
    }
}