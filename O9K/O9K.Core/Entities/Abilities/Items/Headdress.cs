﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_headdress)]
public class Headdress : PassiveAbility
{
    public Headdress(Ability baseAbility)
        : base(baseAbility)
    {
    }
}