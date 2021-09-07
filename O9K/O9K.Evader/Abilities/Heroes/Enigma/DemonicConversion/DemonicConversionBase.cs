﻿namespace O9K.Evader.Abilities.Heroes.Enigma.DemonicConversion;

using Base;
using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.enigma_demonic_conversion)]
internal class DemonicConversionBase : EvaderBaseAbility, IUsable<CounterAbility>
{
    public DemonicConversionBase(Ability9 ability)
        : base(ability)
    {
    }

    public CounterAbility GetUsableAbility()
    {
        return new DemonicConversionUsable(this.Ability, this.Menu);
    }
}