﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Types;
using Divine.Numerics;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Entities.Units;

using Metadata;

[AbilityId(AbilityId.item_glimmer_cape)]
public class GlimmerCape : RangedAbility, IShield
{
    public GlimmerCape(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public UnitState AppliesUnitState { get; } = 0;

    public override bool IsInvisibility { get; } = true;

    public string ShieldModifierName { get; } = "modifier_item_glimmer_cape_fade";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;

    public override float GetCastDelay(Unit9 unit)
    {
        return this.GetCastDelay();
    }

    public override float GetCastDelay(Vector3 position)
    {
        return this.GetCastDelay();
    }
}