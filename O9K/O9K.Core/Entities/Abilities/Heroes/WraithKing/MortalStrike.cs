﻿namespace O9K.Core.Entities.Abilities.Heroes.WraithKing;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.skeleton_king_mortal_strike)]
public class MortalStrike : ActiveAbility
{
    public MortalStrike(Ability baseAbility)
        : base(baseAbility)
    {
    }
}