﻿namespace O9K.Core.Entities.Abilities.Heroes.ElderTitan;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.elder_titan_return_spirit)]
public class ReturnAstralSpirit : ActiveAbility
{
    public ReturnAstralSpirit(Ability baseAbility)
        : base(baseAbility)
    {
    }
}