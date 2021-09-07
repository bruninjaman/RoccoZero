﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_sange_and_yasha)]
public class SangeAndYasha : PassiveAbility
{
    public SangeAndYasha(Ability baseAbility)
        : base(baseAbility)
    {
    }
}