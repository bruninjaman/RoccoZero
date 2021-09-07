﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_ring_of_protection)]
public class RingOfProtection : PassiveAbility
{
    public RingOfProtection(Ability baseAbility)
        : base(baseAbility)
    {
    }
}