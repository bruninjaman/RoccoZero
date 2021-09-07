﻿namespace O9K.Core.Entities.Abilities.Heroes.Tinker;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.tinker_laser)]
public class Laser : RangedAbility, INuke
{
    public Laser(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "laser_damage");
    }
}