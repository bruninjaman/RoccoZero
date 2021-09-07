﻿namespace O9K.Core.Entities.Abilities.Units.Roshan;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.roshan_spell_block)]
public class SpellBlock : PassiveAbility
{
    public SpellBlock(Ability baseAbility)
        : base(baseAbility)
    {
    }
}