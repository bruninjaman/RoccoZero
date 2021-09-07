﻿namespace O9K.Core.Entities.Abilities.Heroes.SandKing;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.sandking_epicenter)]
public class Epicenter : AreaOfEffectAbility, IChanneled
{
    public Epicenter(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "epicenter_damage");
        this.ChannelTime = baseAbility.AbilityData.GetChannelMaximumTime(0);
    }

    public float ChannelTime { get; }

    public bool IsActivatesOnChannelStart { get; } = false;

    public override float Radius { get; } = 350;
}