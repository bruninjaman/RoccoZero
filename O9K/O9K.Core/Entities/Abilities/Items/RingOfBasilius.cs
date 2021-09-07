﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_ring_of_basilius)]
public class RingOfBasilius : ToggleAbility
{
    public RingOfBasilius(Ability baseAbility)
        : base(baseAbility)
    {
    }
}