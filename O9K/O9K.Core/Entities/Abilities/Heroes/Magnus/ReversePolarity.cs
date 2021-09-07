﻿namespace O9K.Core.Entities.Abilities.Heroes.Magnus;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.magnataur_reverse_polarity)]
public class ReversePolarity : AreaOfEffectAbility, IDisable
{
    public ReversePolarity(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "pull_radius");
        this.DamageData = new SpecialData(baseAbility, "polarity_damage");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}