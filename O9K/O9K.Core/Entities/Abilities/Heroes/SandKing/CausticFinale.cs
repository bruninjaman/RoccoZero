﻿namespace O9K.Core.Entities.Abilities.Heroes.SandKing;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.sandking_caustic_finale)]
public class CausticFinale : PassiveAbility
{
    public CausticFinale(Ability baseAbility)
        : base(baseAbility)
    {
    }
}