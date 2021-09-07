﻿namespace O9K.Core.Entities.Abilities.Units.AncientBlackDragon;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.black_dragon_fireball)]
public class Fireball : CircleAbility, IHarass
{
    public Fireball(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "damage");
        this.RadiusData = new SpecialData(baseAbility, "radius");
    }
}