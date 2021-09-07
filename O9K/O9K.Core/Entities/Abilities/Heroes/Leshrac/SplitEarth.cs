﻿namespace O9K.Core.Entities.Abilities.Heroes.Leshrac;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.leshrac_split_earth)]
public class SplitEarth : CircleAbility, IDisable, INuke
{
    public SplitEarth(Ability baseAbility)
        : base(baseAbility)
    {
        this.ActivationDelayData = new SpecialData(baseAbility, "delay");
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}