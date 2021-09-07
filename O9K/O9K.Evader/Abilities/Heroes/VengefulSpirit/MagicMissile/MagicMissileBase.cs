﻿namespace O9K.Evader.Abilities.Heroes.VengefulSpirit.MagicMissile;

using Base;
using Base.Evadable;
using Base.Usable.DisableAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.vengefulspirit_magic_missile)]
internal class MagicMissileBase : EvaderBaseAbility, IEvadable, IUsable<DisableAbility>
{
    public MagicMissileBase(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new MagicMissileEvadable(this.Ability, this.Pathfinder, this.Menu);
    }

    public DisableAbility GetUsableAbility()
    {
        return new DisableAbility(this.Ability, this.Menu);
    }
}