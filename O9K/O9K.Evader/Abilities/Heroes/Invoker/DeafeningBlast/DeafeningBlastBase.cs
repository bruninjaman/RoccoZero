﻿namespace O9K.Evader.Abilities.Heroes.Invoker.DeafeningBlast;

using Base;
using Base.Evadable;
using Base.Usable.DisableAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.invoker_deafening_blast)]
internal class DeafeningBlastBase : EvaderBaseAbility, IEvadable, IUsable<DisableAbility>
{
    public DeafeningBlastBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new DeafeningBlastEvadable(this.Ability, this.Pathfinder, this.Menu);
    }

    public DisableAbility GetUsableAbility()
    {
        return new DisableAbility(this.Ability, this.Menu);
    }
}