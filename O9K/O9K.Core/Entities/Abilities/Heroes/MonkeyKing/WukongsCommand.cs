﻿namespace O9K.Core.Entities.Abilities.Heroes.MonkeyKing;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.monkey_king_wukongs_command)]
public class WukongsCommand : CircleAbility
{
    private SpecialData castRangeData;

    public WukongsCommand(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "second_radius");
        this.castRangeData = new SpecialData(baseAbility, "cast_range");
    }
}