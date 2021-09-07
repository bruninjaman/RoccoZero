﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.item_lotus_orb)]
public class LotusOrb : RangedAbility, IShield
{
    public LotusOrb(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = 0;

    public string ShieldModifierName { get; } = "modifier_item_lotus_orb_active";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;
}