﻿namespace O9K.Core.Entities.Abilities.Heroes.Meepo;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.meepo_poof)]
public class Poof : RangedAbility, INuke
{
    public Poof(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.DamageData = new SpecialData(baseAbility, "poof_damage");
    }

    public override float CastRange { get; } = 9999999;
}