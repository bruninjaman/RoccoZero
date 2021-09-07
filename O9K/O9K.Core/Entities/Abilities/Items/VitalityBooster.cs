namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_vitality_booster)]
public class VitalityBooster : PassiveAbility
{
    public VitalityBooster(Ability baseAbility)
        : base(baseAbility)
    {
    }
}