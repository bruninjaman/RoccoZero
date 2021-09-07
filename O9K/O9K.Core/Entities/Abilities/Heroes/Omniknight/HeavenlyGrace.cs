﻿namespace O9K.Core.Entities.Abilities.Heroes.Omniknight;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.omniknight_repel)]
public class HeavenlyGrace : RangedAbility, IShield
{
    public HeavenlyGrace(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = 0;

    public string ShieldModifierName { get; } = "modifier_omniknight_repel";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;
}