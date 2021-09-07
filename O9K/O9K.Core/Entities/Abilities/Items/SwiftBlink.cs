﻿namespace O9K.Core.Entities.Abilities.Items;

using System;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.item_swift_blink)]
public class SwiftBlink : RangedAbility, IBlink
{
    private readonly SpecialData castRangeData;

    public SwiftBlink(Ability baseAbility)
        : base(baseAbility)
    {
        this.castRangeData = new SpecialData(baseAbility, "blink_range");
    }

    public BlinkType BlinkType { get; } = BlinkType.Blink;

    public override float TimeSinceCasted
    {
        get
        {
            //prevent damage 3s cd to consider as casted
            var cooldownLength = this.BaseAbility.CooldownLength;
            return cooldownLength <= 0 ? 9999999 : Math.Max(cooldownLength, 10) - this.BaseAbility.Cooldown;
        }
    }

    protected override float BaseCastRange
    {
        get
        {
            return this.castRangeData.GetValue(this.Level) - 25;
        }
    }
}