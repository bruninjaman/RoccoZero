﻿namespace O9K.Evader.Abilities.Heroes.Doom.InfernalBlade;

using Base;
using Base.Usable.DisableAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.doom_bringer_infernal_blade)]
internal class InfernalBladeBase : EvaderBaseAbility, IUsable<DisableAbility>
{
    public InfernalBladeBase(Ability9 ability)
        : base(ability)
    {
    }

    public DisableAbility GetUsableAbility()
    {
        return new DisableAbility(this.Ability, this.Menu);
    }
}