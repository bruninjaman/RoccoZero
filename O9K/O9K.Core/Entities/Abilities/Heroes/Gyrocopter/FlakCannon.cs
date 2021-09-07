namespace O9K.Core.Entities.Abilities.Heroes.Gyrocopter;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.gyrocopter_flak_cannon)]
public class FlakCannon : AreaOfEffectAbility
{
    public FlakCannon(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }
}