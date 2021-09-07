﻿namespace O9K.Core.Entities.Abilities.Heroes.EarthSpirit;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.earth_spirit_magnetize)]
public class Magnetize : AreaOfEffectAbility
{
    public Magnetize(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "cast_radius");
    }

    public override float CastRange { get; } = 0;
}