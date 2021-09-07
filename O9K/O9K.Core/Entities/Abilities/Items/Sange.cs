namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_sange)]
public class Sange : PassiveAbility
{
    public Sange(Ability baseAbility)
        : base(baseAbility)
    {
    }
}