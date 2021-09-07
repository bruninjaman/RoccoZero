﻿namespace O9K.Core.Entities.Abilities.Heroes.ElderTitan;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.elder_titan_echo_stomp)]
public class EchoStomp : AreaOfEffectAbility, IChanneled, IDisable, IAppliesImmobility
{
    public EchoStomp(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.ActivationDelayData = new SpecialData(baseAbility, "cast_time");
        this.DamageData = new SpecialData(baseAbility, "stomp_damage");
        this.ChannelTime = baseAbility.AbilityData.GetChannelMaximumTime(0);
    }

    public override float ActivationDelay
    {
        get
        {
            return base.ActivationDelay - this.CastPoint;
        }
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;

    public float ChannelTime { get; }

    public string ImmobilityModifierName { get; } = "modifier_elder_titan_echo_stomp";

    public bool IsActivatesOnChannelStart { get; } = false;
}