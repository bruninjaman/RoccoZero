﻿namespace O9K.Evader.Abilities.Heroes.Tiny.TreeThrow;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.tiny_toss_tree)]
internal class TreeThrowBase : EvaderBaseAbility, IEvadable
{
    public TreeThrowBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new TreeThrowEvadable(this.Ability, this.Pathfinder, this.Menu);
    }
}