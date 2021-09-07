﻿namespace O9K.Evader.Abilities.Heroes.AntiMage.Counterspell;

using Base;
using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.antimage_counterspell)]
internal class CounterspellBase : EvaderBaseAbility, IUsable<CounterAbility>
{
    public CounterspellBase(Ability9 ability)
        : base(ability)
    {
    }

    public CounterAbility GetUsableAbility()
    {
        return new CounterspellUsable(this.Ability, this.Menu);
    }
}