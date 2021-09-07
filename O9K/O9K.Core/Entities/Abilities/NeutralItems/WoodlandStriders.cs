﻿namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_woodland_striders)]
public class WoodlandStriders : ActiveAbility
{
    public WoodlandStriders(Ability baseAbility)
        : base(baseAbility)
    {
    }
}