﻿namespace O9K.Core.Entities.Abilities.Heroes.SandKing;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.sandking_sand_storm)]
public class SandStorm : AreaOfEffectAbility
{
    public SandStorm(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "sand_storm_radius");
        this.DamageData = new SpecialData(baseAbility, "sand_storm_damage");
    }

    public override bool IsInvisibility { get; } = true;
}