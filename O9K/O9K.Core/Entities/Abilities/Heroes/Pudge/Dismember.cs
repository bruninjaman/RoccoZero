﻿namespace O9K.Core.Entities.Abilities.Heroes.Pudge;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.pudge_dismember)]
public class Dismember : RangedAbility, IChanneled, IDisable
{
    private readonly SpecialData channelTimeData;

    public Dismember(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "dismember_damage");
        this.channelTimeData = new SpecialData(baseAbility, "AbilityChannelTime");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;

    public override float CastRange
    {
        get
        {
            return base.CastRange + 100;
        }
    }

    public float ChannelTime
    {
        get
        {
            return this.channelTimeData.GetValue(this.Level);
        }
    }

    public bool IsActivatesOnChannelStart { get; } = true;
}