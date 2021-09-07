﻿namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_spell_prism)]
public class SpellPrism : PassiveAbility
{
    public SpellPrism(Ability baseAbility)
        : base(baseAbility)
    {
    }
}