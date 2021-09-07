﻿namespace O9K.Core.Entities.Abilities.Heroes.Grimstroke;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId((AbilityId)7852)]
//[AbilityId(AbilityId.grimstroke_scepter)]
public class DarkPortrait : RangedAbility, IHarass
{
    public DarkPortrait(Ability baseAbility)
        : base(baseAbility)
    {
    }
}