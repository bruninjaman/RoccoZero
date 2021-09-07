﻿namespace O9K.Core.Entities.Abilities.Heroes.Techies;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.techies_minefield_sign)]
public class MinefieldSign : CircleAbility
{
    public MinefieldSign(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "aura_radius");
    }
}