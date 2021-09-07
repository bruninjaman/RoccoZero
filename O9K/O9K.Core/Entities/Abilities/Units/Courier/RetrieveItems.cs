﻿namespace O9K.Core.Entities.Abilities.Units.Courier;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.courier_take_stash_items)]
public class RetrieveItems : ActiveAbility
{
    public RetrieveItems(Ability baseAbility)
        : base(baseAbility)
    {
    }
}