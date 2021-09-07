﻿namespace O9K.Core.Entities.Abilities.Heroes.Techies;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.techies_focused_detonate)]
public class FocusedDetonate : CircleAbility
{
    public FocusedDetonate(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }

    public override float CastRange { get; } = 9999999;
}