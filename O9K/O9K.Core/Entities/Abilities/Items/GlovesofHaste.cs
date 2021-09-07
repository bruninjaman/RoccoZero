namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_gloves)]
public class GlovesOfHaste : PassiveAbility
{
    public GlovesOfHaste(Ability baseAbility)
        : base(baseAbility)
    {
    }
}