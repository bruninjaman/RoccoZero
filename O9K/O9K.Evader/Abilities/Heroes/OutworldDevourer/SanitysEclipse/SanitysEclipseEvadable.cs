﻿namespace O9K.Evader.Abilities.Heroes.OutworldDevourer.SanitysEclipse;

using Base.Evadable;

using Core.Entities.Abilities.Base;

using Metadata;

internal sealed class SanitysEclipseEvadable : LinearAreaOfEffectEvadable
{
    public SanitysEclipseEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Disables.UnionWith(Abilities.Disable);

        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.Mischief);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.Invulnerability);
        this.Counters.UnionWith(Abilities.StrongMagicShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.Add(Abilities.BladeMail);
    }
}