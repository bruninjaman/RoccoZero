﻿namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_quickening_charm)]
public class QuickeningCharm : PassiveAbility
{
    public QuickeningCharm(Ability baseAbility)
        : base(baseAbility)
    {
    }
}