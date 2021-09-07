﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.item_sheepstick)]
public class ScytheOfVyse : RangedAbility, IDisable, IAppliesImmobility
{
    public ScytheOfVyse(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = UnitState.Hexed | UnitState.Silenced | UnitState.Disarmed;

    public string ImmobilityModifierName { get; } = "modifier_sheepstick_debuff";
}