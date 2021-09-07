﻿namespace O9K.Core.Entities.Abilities.Heroes.WraithKing;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.skeleton_king_vampiric_aura)]
public class VampiricAura : PassiveAbility
{
    public VampiricAura(Ability baseAbility)
        : base(baseAbility)
    {
    }
}