﻿namespace O9K.Core.Entities.Abilities.Heroes.CrystalMaiden;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.crystal_maiden_frostbite)]
public class Frostbite : RangedAbility, IDisable, IAppliesImmobility
{
    public Frostbite(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = UnitState.Rooted;

    public string ImmobilityModifierName { get; } = "modifier_crystal_maiden_frostbite";
}