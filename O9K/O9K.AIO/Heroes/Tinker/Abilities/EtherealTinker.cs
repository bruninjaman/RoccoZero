﻿namespace O9K.AIO.Heroes.Tinker.Abilities;


using AIO.Abilities;

using Core.Entities.Abilities.Base;

using O9K.Core.Entities.Units;

internal class EtherealTinker : DisableAbility
{
    public EtherealTinker(ActiveAbility ability)
        : base(ability)
    {
    }

    protected override bool ChainStun(Unit9 target, bool invulnerability)
    {
        return true;
    }
}