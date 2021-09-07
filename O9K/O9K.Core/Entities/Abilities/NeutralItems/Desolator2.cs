﻿namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_desolator_2)]
public class Desolator2 : PassiveAbility
{
    public Desolator2(Ability baseAbility)
        : base(baseAbility)
    {
    }
}