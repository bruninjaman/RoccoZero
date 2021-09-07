﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_belt_of_strength)]
public class BeltOfStrength : PassiveAbility
{
    public BeltOfStrength(Ability baseAbility)
        : base(baseAbility)
    {
    }
}