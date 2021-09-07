﻿namespace O9K.Core.Entities.Abilities.Heroes.Brewmaster;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.brewmaster_drunken_brawler)]
public class DrunkenBrawler : ActiveAbility, IBuff
{
    public DrunkenBrawler(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public string BuffModifierName { get; } = "modifier_brewmaster_drunken_brawler";

    public bool BuffsAlly { get; } = false;

    public bool BuffsOwner { get; } = true;
}