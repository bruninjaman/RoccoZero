﻿namespace O9K.Evader.Abilities.Heroes.DeathProphet.Exorcism;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.death_prophet_exorcism)]
internal class ExorcismBase : EvaderBaseAbility, IEvadable
{
    public ExorcismBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new ExorcismEvadable(this.Ability, this.Pathfinder, this.Menu);
    }
}