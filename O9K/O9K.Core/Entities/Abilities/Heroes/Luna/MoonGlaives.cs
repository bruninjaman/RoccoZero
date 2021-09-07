﻿namespace O9K.Core.Entities.Abilities.Heroes.Luna;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.luna_moon_glaive)]
public class MoonGlaives : PassiveAbility
{
    public MoonGlaives(Ability baseAbility)
        : base(baseAbility)
    {
    }
}