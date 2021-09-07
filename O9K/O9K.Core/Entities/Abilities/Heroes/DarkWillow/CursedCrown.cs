﻿namespace O9K.Core.Entities.Abilities.Heroes.DarkWillow;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.dark_willow_cursed_crown)]
public class CursedCrown : RangedAbility, IDebuff, IDisable
{
    public CursedCrown(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "stun_radius");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;

    public string DebuffModifierName { get; } = "modifier_dark_willow_cursed_crown";
}