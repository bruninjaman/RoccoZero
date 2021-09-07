﻿namespace O9K.Core.Entities.Abilities.Heroes.DrowRanger;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.drow_ranger_marksmanship)]
public class Marksmanship : PassiveAbility
{
    public Marksmanship(Ability baseAbility)
        : base(baseAbility)
    {
    }
}