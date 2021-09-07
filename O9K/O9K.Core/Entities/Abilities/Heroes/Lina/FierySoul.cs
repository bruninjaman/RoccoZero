﻿namespace O9K.Core.Entities.Abilities.Heroes.Lina;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.lina_fiery_soul)]
public class FierySoul : PassiveAbility
{
    public FierySoul(Ability baseAbility)
        : base(baseAbility)
    {
    }
}