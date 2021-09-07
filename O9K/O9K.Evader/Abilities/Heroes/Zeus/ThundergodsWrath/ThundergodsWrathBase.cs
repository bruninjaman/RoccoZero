﻿namespace O9K.Evader.Abilities.Heroes.Zeus.ThundergodsWrath;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.zuus_thundergods_wrath)]
internal class ThundergodsWrathBase : EvaderBaseAbility, IEvadable
{
    public ThundergodsWrathBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new ThundergodsWrathEvadable(this.Ability, this.Pathfinder, this.Menu);
    }
}