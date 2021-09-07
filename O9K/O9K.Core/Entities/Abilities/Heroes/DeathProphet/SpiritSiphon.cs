﻿namespace O9K.Core.Entities.Abilities.Heroes.DeathProphet;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.death_prophet_spirit_siphon)]
public class SpiritSiphon : RangedAbility, IDebuff
{
    public SpiritSiphon(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public string DebuffModifierName { get; } = "modifier_death_prophet_spirit_siphon_slow";
}