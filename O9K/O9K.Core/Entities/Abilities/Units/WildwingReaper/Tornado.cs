﻿namespace O9K.Core.Entities.Abilities.Units.WildwingReaper;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.enraged_wildkin_tornado)]
public class Tornado : RangedAbility
{
    public Tornado(Ability baseAbility)
        : base(baseAbility)
    {
    }
}