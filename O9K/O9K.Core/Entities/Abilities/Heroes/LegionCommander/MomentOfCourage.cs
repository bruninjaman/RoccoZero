﻿namespace O9K.Core.Entities.Abilities.Heroes.LegionCommander;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.legion_commander_moment_of_courage)]
public class MomentOfCourage : PassiveAbility
{
    public MomentOfCourage(Ability baseAbility)
        : base(baseAbility)
    {
    }
}