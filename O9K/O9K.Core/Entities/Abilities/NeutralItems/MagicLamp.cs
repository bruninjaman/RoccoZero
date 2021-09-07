﻿namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_panic_button)]
public class MagicLamp : PassiveAbility
{
    public MagicLamp(Ability baseAbility)
        : base(baseAbility)
    {
    }
}