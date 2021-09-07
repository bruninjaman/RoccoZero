﻿namespace O9K.Core.Entities.Abilities.Heroes.Earthshaker;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.earthshaker_aftershock)]
public class Aftershock : PassiveAbility, IHasRadius
{
    public Aftershock(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "aftershock_range");
    }
}