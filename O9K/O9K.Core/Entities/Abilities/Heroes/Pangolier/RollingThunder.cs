﻿namespace O9K.Core.Entities.Abilities.Heroes.Pangolier;

using Base;
using Divine.Numerics;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.pangolier_gyroshell)]
public class RollingThunder : ActiveAbility
{
    private readonly SpecialData turnRateData;

    public RollingThunder(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "hit_radius");
        this.SpeedData = new SpecialData(baseAbility, "forward_move_speed");
        this.turnRateData = new SpecialData(baseAbility, "turn_rate");
    }

    public float TurnRate
    {
        get
        {
            return MathUtil.DegreesToRadians(this.turnRateData.GetValue(this.Level));
        }
    }
}