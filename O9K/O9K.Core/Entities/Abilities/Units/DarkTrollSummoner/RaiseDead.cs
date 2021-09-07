﻿namespace O9K.Core.Entities.Abilities.Units.DarkTrollSummoner;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.dark_troll_warlord_raise_dead)]
public class RaiseDead : ActiveAbility
{
    public RaiseDead(Ability baseAbility)
        : base(baseAbility)
    {
    }
}