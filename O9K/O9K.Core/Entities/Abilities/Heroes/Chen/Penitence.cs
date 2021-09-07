﻿namespace O9K.Core.Entities.Abilities.Heroes.Chen;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.chen_penitence)]
public class Penitence : RangedAbility, IDebuff
{
    public Penitence(Ability baseAbility)
        : base(baseAbility)
    {
        this.SpeedData = new SpecialData(baseAbility, "speed");
    }

    public string DebuffModifierName { get; } = "modifier_chen_penitence";
}