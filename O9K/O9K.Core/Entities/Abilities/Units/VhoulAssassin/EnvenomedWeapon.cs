﻿namespace O9K.Core.Entities.Abilities.Units.VhoulAssassin;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.gnoll_assassin_envenomed_weapon)]
public class EnvenomedWeapon : PassiveAbility
{
    public EnvenomedWeapon(Ability baseAbility)
        : base(baseAbility)
    {
    }
}