﻿namespace O9K.Evader.Abilities.Heroes.EarthSpirit.Magnetize;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class MagnetizeEvadable : ModifierCounterEvadable
{
    public MagnetizeEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierCounters.Add(Abilities.MantaStyle);
        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
        this.ModifierCounters.UnionWith(Abilities.StrongMagicShield);
    }

    public override bool ModifierAllyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}