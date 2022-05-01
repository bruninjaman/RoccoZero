namespace O9K.Core.Entities.Abilities.Heroes.Bloodseeker;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.bloodseeker_blood_mist)]
public class BloodMist : ToggleAbility
{
    public BloodMist(Ability baseAbility)
        : base(baseAbility)
    {
    }
}