﻿namespace O9K.Evader.Abilities.Heroes.Enchantress.Spoink;

using Base;
using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.enchantress_bunny_hop)]
internal class SpoinkBase : EvaderBaseAbility, IUsable<CounterAbility>
{
    public SpoinkBase(Ability9 ability)
        : base(ability)
    {
    }

    public CounterAbility GetUsableAbility()
    {
        return new SpoinkUsable(this.Ability, this.Menu);
    }
}