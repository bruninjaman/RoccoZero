﻿namespace O9K.Evader.Abilities.Heroes.Luna.LucentBeam;

using Base.Evadable;

using Core.Entities.Abilities.Base;

using Metadata;

internal sealed class LucentBeamEvadable : TargetableEvadable
{
    public LucentBeamEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.Counterspell);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.Shield);
        this.Counters.UnionWith(Abilities.MagicShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);
        this.Counters.Add(Abilities.LotusOrb);
    }
}