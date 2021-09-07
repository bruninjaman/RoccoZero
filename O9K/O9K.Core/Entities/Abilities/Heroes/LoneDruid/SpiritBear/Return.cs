﻿namespace O9K.Core.Entities.Abilities.Heroes.LoneDruid.SpiritBear;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.lone_druid_spirit_bear_return)]
public class Return : ActiveAbility, IChanneled
{
    public Return(Ability baseAbility)
        : base(baseAbility)
    {
        this.ChannelTime = baseAbility.AbilityData.GetChannelMaximumTime(0);
    }

    public float ChannelTime { get; }

    public bool IsActivatesOnChannelStart { get; } = false;
}