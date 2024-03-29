﻿namespace O9K.Evader.Abilities.Heroes.Axe.CullingBlade;

using Base.Evadable;

using Core.Entities.Abilities.Base;

using Metadata;

internal sealed class CullingBladeEvadable : TargetableEvadable
{
    public CullingBladeEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Disables.UnionWith(Abilities.Disable);

        this.Counters.Add(Abilities.Counterspell);
        this.Counters.Add(Abilities.LinkensSphere);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.Mischief);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.Shield);
        this.Counters.UnionWith(Abilities.Invulnerability);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.Add(Abilities.HurricanePike);
        this.Counters.Add(Abilities.Dispose);
        this.Counters.Add(Abilities.PsychicHeadband);
        this.Counters.Add(Abilities.CourierShield);

        this.Counters.Remove(Abilities.BladeMail);
        this.Counters.Remove(Abilities.Nightmare);
        this.Counters.Remove(Abilities.SpikedCarapace);
    }
}