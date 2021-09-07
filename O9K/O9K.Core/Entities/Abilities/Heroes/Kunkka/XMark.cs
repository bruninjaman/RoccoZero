﻿namespace O9K.Core.Entities.Abilities.Heroes.Kunkka;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.kunkka_x_marks_the_spot)]
public class XMark : RangedAbility
{
    public XMark(Ability baseAbility)
        : base(baseAbility)
    {
    }
}