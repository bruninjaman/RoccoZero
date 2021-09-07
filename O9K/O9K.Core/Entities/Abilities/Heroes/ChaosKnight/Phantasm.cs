﻿namespace O9K.Core.Entities.Abilities.Heroes.ChaosKnight;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.chaos_knight_phantasm)]
public class Phantasm : ActiveAbility
{
    public Phantasm(Ability baseAbility)
        : base(baseAbility)
    {
    }
}