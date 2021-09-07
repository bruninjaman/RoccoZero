﻿namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_royal_jelly)]
public class RoyalJelly : RangedAbility
{
    public RoyalJelly(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override float CastRange { get; } = 9999999;
}