﻿namespace O9K.Core.Entities.Abilities.Heroes.Slark;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.slark_shadow_dance)]
public class ShadowDance : ActiveAbility, IShield
{
    public ShadowDance(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = UnitState.Untargetable;

    public string ShieldModifierName { get; } = "modifier_slark_shadow_dance";

    public bool ShieldsAlly { get; } = false;

    public bool ShieldsOwner { get; } = true;
}