﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_manta)]
public class MantaStyle : ActiveAbility, IBuff
{
    public MantaStyle(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public string BuffModifierName { get; } = string.Empty;

    public bool BuffsAlly { get; } = false;

    public bool BuffsOwner { get; } = true;
}