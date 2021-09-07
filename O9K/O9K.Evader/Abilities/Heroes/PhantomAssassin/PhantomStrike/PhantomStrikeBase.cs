﻿namespace O9K.Evader.Abilities.Heroes.PhantomAssassin.PhantomStrike;

using Base;
using Base.Evadable;
using Base.Usable.BlinkAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.phantom_assassin_phantom_strike)]
internal class PhantomStrikeBase : EvaderBaseAbility, IEvadable, IUsable<BlinkAbility>
{
    public PhantomStrikeBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new PhantomStrikeEvadable(this.Ability, this.Pathfinder, this.Menu);
    }

    public BlinkAbility GetUsableAbility()
    {
        return new BlinkTargetableAbility(this.Ability, this.Pathfinder, this.Menu);
    }
}