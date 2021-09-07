﻿namespace O9K.Core.Entities.Abilities.Heroes.Timbersaw;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.shredder_reactive_armor)]
public class ReactiveArmor : PassiveAbility
{
    public ReactiveArmor(Ability baseAbility)
        : base(baseAbility)
    {
    }
}