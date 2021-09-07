﻿namespace O9K.Core.Entities.Abilities.Units.MudGolem;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.mud_golem_rock_destroy)]
public class ShardSplit : PassiveAbility
{
    public ShardSplit(Ability baseAbility)
        : base(baseAbility)
    {
    }
}