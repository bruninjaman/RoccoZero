﻿namespace O9K.Core.Entities.Abilities.Heroes.Brewmaster;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.brewmaster_thunder_clap)]
public class ThunderClap : AreaOfEffectAbility, INuke
{
    public ThunderClap(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }
}