﻿namespace O9K.Core.Entities.Abilities.Heroes.Snapfire;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.snapfire_gobble_up)]
public class GobbleUp : RangedAbility
{
    public GobbleUp(Ability baseAbility)
        : base(baseAbility)
    {
    }
}