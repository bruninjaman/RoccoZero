﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_lesser_crit)]
public class Crystalys : PassiveAbility
{
    public Crystalys(Ability baseAbility)
        : base(baseAbility)
    {
    }
}