﻿namespace O9K.Core.Entities.Abilities.Heroes.Slardar;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.slardar_amplify_damage)]
public class CorrosiveHaze : RangedAbility, IDebuff
{
    public CorrosiveHaze(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public string DebuffModifierName { get; } = "modifier_slardar_amplify_damage";
}