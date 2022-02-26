namespace O9K.Core.Entities.Abilities.Heroes.Beastmaster;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.beastmaster_call_of_the_wild_hawk)]
public class CallOfTheWildHawk : CircleAbility
{
    public CallOfTheWildHawk(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "hawk_vision_tooltip");
        this.SpeedData = new SpecialData(baseAbility, "hawk_speed_tooltip");
    }

    public override float CastRange { get; } = 9999999;
}