﻿namespace O9K.Core.Entities.Abilities.Heroes.ShadowFiend;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.nevermore_dark_lord)]
public class PresenceOfTheDarkLord : PassiveAbility
{
    public PresenceOfTheDarkLord(Ability baseAbility)
        : base(baseAbility)
    {
    }
}