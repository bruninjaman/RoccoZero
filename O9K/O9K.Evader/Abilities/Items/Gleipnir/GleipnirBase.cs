﻿namespace O9K.Evader.Abilities.Items.Gleipnir;

using Base;
using Base.Evadable;
using Base.Usable.DisableAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.item_gungir)]
internal class GleipnirBase : EvaderBaseAbility, IEvadable, IUsable<DisableAbility>
{
    public GleipnirBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new GleipnirEvadable(this.Ability, this.Pathfinder, this.Menu);
    }

    public DisableAbility GetUsableAbility()
    {
        return new DisableAbility(this.Ability, this.Menu);
    }
}