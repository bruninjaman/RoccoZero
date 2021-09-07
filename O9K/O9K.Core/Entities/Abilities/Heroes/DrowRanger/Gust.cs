﻿namespace O9K.Core.Entities.Abilities.Heroes.DrowRanger;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.drow_ranger_wave_of_silence)]
public class Gust : LineAbility, IDisable
{
    public Gust(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "wave_width");
        this.SpeedData = new SpecialData(baseAbility, "wave_speed");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Silenced;
}