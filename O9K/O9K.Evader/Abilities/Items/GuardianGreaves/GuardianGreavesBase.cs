﻿namespace O9K.Evader.Abilities.Items.GuardianGreaves;

using Base;
using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.item_guardian_greaves)]
internal class GuardianGreavesBase : EvaderBaseAbility, IUsable<CounterAbility>
{
    public GuardianGreavesBase(Ability9 ability)
        : base(ability)
    {
    }

    public CounterAbility GetUsableAbility()
    {
        // return new CounterAbility(this.Ability, this.Menu);
        return new CounterHealAbility(this.Ability, this.Menu);
    }
}