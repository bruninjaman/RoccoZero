﻿namespace O9K.Core.Entities.Abilities.Heroes.Lich;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.lich_frost_nova)]
public class FrostBlast : RangedAreaOfEffectAbility, INuke
{
    public FrostBlast(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }
}