﻿namespace O9K.Core.Entities.Abilities.Heroes.DarkWillow;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.dawnbreaker_luminosity)]
public class Luminosity : PassiveAbility
{
    public Luminosity(Ability baseAbility)
        : base(baseAbility)
    {
    }
}