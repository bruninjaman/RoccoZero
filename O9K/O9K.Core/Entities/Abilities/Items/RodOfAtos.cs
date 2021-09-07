﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.item_rod_of_atos)]
public class RodOfAtos : RangedAbility, IDisable, IAppliesImmobility
{
    public RodOfAtos(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = UnitState.Rooted;

    public string ImmobilityModifierName { get; } = "modifier_rod_of_atos_debuff";

    public override float Speed { get; } = 1750;
}