﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_robe)]
public class RobeoftheMagi : PassiveAbility
{
    public RobeoftheMagi(Ability baseAbility)
        : base(baseAbility)
    {
    }
}