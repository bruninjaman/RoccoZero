﻿namespace O9K.Core.Entities.Abilities.Heroes.Phoenix;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.phoenix_sun_ray_stop)]
public class SunRayStop : ActiveAbility
{
    public SunRayStop(Ability baseAbility)
        : base(baseAbility)
    {
    }
}