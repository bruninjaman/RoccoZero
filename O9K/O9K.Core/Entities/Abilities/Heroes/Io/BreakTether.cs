﻿namespace O9K.Core.Entities.Abilities.Heroes.Io;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.wisp_tether_break)]
public class BreakTether : ActiveAbility
{
    public BreakTether(Ability baseAbility)
        : base(baseAbility)
    {
    }
}