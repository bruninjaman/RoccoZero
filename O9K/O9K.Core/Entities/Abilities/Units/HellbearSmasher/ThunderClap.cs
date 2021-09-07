﻿namespace O9K.Core.Entities.Abilities.Units.HellbearSmasher;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.polar_furbolg_ursa_warrior_thunder_clap)]
public class ThunderClap : AreaOfEffectAbility, INuke
{
    public ThunderClap(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }
}