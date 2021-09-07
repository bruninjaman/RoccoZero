﻿namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_trusty_shovel)]
public class TrustyShovel : RangedAbility, IChanneled
{
    public TrustyShovel(Ability baseAbility)
        : base(baseAbility)
    {
        this.ChannelTime = baseAbility.AbilityData.GetChannelMaximumTime(0);
    }

    public float ChannelTime { get; }

    public bool IsActivatesOnChannelStart { get; } = false;
}