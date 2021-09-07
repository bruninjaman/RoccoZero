﻿namespace O9K.Core.Entities.Abilities.Heroes.QueenOfPain;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.queenofpain_blink)]
public class Blink : RangedAbility, IBlink
{
    private readonly SpecialData castRangeData;

    public Blink(Ability baseAbility)
        : base(baseAbility)
    {
        this.castRangeData = new SpecialData(baseAbility, "blink_range");
    }

    public BlinkType BlinkType { get; } = BlinkType.Blink;

    protected override float BaseCastRange
    {
        get
        {
            return this.castRangeData.GetValue(this.Level);
        }
    }
}