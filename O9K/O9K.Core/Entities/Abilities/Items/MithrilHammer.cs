﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_mithril_hammer)]
public class MithrilHammer : PassiveAbility
{
    public MithrilHammer(Ability baseAbility)
        : base(baseAbility)
    {
    }
}