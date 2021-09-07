﻿namespace O9K.Core.Entities.Abilities.Heroes.Broodmother;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.broodmother_spawn_spiderlings)]
public class SpawnSpiderlings : RangedAbility, INuke
{
    public SpawnSpiderlings(Ability baseAbility)
        : base(baseAbility)
    {
        this.SpeedData = new SpecialData(baseAbility, "projectile_speed");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }
}