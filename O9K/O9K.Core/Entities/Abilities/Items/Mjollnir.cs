﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.item_mjollnir)]
public class Mjollnir : RangedAbility, IShield
{
    public Mjollnir(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = 0;

    public string ShieldModifierName { get; } = "modifier_item_mjollnir_static";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;
}