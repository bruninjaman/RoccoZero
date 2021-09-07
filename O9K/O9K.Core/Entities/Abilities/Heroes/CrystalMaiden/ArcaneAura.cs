﻿namespace O9K.Core.Entities.Abilities.Heroes.CrystalMaiden;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.crystal_maiden_brilliance_aura)]
public class ArcaneAura : PassiveAbility
{
    public ArcaneAura(Ability baseAbility)
        : base(baseAbility)
    {
    }
}