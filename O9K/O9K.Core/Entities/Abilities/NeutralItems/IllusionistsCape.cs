﻿namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_illusionsts_cape)]
public class IllusionistsCape : ActiveAbility, IBuff
{
    public IllusionistsCape(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public string BuffModifierName { get; } = string.Empty;

    public bool BuffsAlly { get; } = false;

    public bool BuffsOwner { get; } = true;
}