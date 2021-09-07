﻿namespace O9K.Core.Entities.Abilities.Heroes.Doom;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.doom_bringer_devour)]
public class Devour : RangedAbility
{
    public Devour(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override bool TargetsEnemy { get; } = false;
}