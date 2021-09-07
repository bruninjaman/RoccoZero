﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.item_blade_mail)]
public class BladeMail : ActiveAbility, IShield
{
    public BladeMail(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = 0;

    public string ShieldModifierName { get; } = "modifier_item_blade_mail_reflect";

    public bool ShieldsAlly { get; } = false;

    public bool ShieldsOwner { get; } = true;
}