﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_cloak)]
public class Cloak : PassiveAbility
{
    public Cloak(Ability baseAbility)
        : base(baseAbility)
    {
    }
}