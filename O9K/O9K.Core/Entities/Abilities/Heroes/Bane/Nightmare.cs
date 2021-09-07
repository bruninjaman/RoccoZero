﻿namespace O9K.Core.Entities.Abilities.Heroes.Bane;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.bane_nightmare)]
public class Nightmare : RangedAbility, IDisable, IShield
{
    public Nightmare(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;

    public string ShieldModifierName { get; } = "modifier_bane_nightmare";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;
}