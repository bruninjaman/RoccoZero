﻿namespace O9K.Core.Entities.Abilities.Units.Courier;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.courier_return_stash_items)]
public class ReturnItems : ActiveAbility
{
    public ReturnItems(Ability baseAbility)
        : base(baseAbility)
    {
    }
}