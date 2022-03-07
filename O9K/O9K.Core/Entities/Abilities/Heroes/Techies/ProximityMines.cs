﻿using System;

namespace O9K.Core.Entities.Abilities.Heroes.Techies;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.techies_land_mines)]
public class ProximityMines : CircleAbility
{
    public ProximityMines(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.ActivationDelayData = new SpecialData(baseAbility, "activation_delay");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }
}