﻿namespace O9K.Evader.Abilities.Heroes.Earthshaker.EnchantTotem;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.earthshaker_enchant_totem)]
internal class EnchantTotemBase : EvaderBaseAbility, IEvadable
{
    public EnchantTotemBase(Ability9 ability)
        : base(ability)
    {
        //todo fix aghs totem
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new EnchantTotemEvadable(this.Ability, this.Pathfinder, this.Menu);
    }
}