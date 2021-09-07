﻿namespace O9K.Evader.Abilities.Heroes.Tidehunter.Gush;

using Base;
using Base.Evadable;
using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.tidehunter_gush)]
internal class GushBase : EvaderBaseAbility , IEvadable, IUsable<CounterEnemyAbility>
{
    public GushBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new GushEvadable(this.Ability, this.Pathfinder, this.Menu);
    }

    public CounterEnemyAbility GetUsableAbility()
    {
        return new CounterEnemyAbility(this.Ability, this.Menu);
    }
}