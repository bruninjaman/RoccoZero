﻿namespace O9K.Core.Entities.Abilities.Heroes.WraithKing;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.skeleton_king_hellfire_blast)]
public class WraithfireBlast : RangedAbility, IDisable, INuke
{
    public WraithfireBlast(Ability baseAbility)
        : base(baseAbility)
    {
        this.SpeedData = new SpecialData(baseAbility, "blast_speed");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}