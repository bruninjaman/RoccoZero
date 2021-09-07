﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_helm_of_iron_will)]
public class HelmOfIronWill : PassiveAbility
{
    public HelmOfIronWill(Ability baseAbility)
        : base(baseAbility)
    {
    }
}