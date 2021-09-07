namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_smoke_of_deceit)]
public class SmokeOfDeceit : AreaOfEffectAbility
{
    public SmokeOfDeceit(Ability ability)
        : base(ability)
    {
    }
}