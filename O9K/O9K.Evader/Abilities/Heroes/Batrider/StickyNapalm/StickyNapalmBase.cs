﻿namespace O9K.Evader.Abilities.Heroes.Batrider.StickyNapalm;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.batrider_sticky_napalm)]
internal class StickyNapalmBase : EvaderBaseAbility, IEvadable
{
    public StickyNapalmBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new StickyNapalmEvadable(this.Ability, this.Pathfinder, this.Menu);
    }
}