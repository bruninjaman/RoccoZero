﻿namespace O9K.Core.Entities.Abilities.Heroes.CrystalMaiden;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.crystal_maiden_freezing_field)]
public class FreezingField : AreaOfEffectAbility, IChanneled
{
    public FreezingField(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.DamageData = new SpecialData(baseAbility, "damage");
        this.ChannelTime = baseAbility.AbilityData.GetChannelMaximumTime(0);
    }

    public float ChannelTime { get; }

    public bool IsActivatesOnChannelStart { get; } = true;
}