﻿using O9K.Core.Entities.Abilities.Base.Types;

namespace O9K.Core.Entities.Abilities.Heroes.Techies;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.techies_suicide)]
public class BlastOff : CircleAbility, INuke
{
    public BlastOff(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.ActivationDelayData = new SpecialData(baseAbility, "duration");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }
}