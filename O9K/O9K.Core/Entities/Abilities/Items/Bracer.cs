﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_bracer)]
public class Bracer : PassiveAbility
{
    public Bracer(Ability baseAbility)
        : base(baseAbility)
    {
    }
}