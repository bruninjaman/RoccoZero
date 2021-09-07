﻿namespace O9K.Core.Entities.Abilities.Heroes.BountyHunter;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.bounty_hunter_jinada)]
public class Jinada : PassiveAbility
{
    public Jinada(Ability baseAbility)
        : base(baseAbility)
    {
    }
}