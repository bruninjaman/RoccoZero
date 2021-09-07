﻿namespace O9K.Core.Entities.Abilities.Heroes.BountyHunter;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.bounty_hunter_wind_walk)]
public class ShadowWalk : ActiveAbility
{
    public ShadowWalk(Ability baseAbility)
        : base(baseAbility)
    {
        this.ActivationDelayData = new SpecialData(baseAbility, "fade_time");
    }

    public override bool IsInvisibility { get; } = true;
}