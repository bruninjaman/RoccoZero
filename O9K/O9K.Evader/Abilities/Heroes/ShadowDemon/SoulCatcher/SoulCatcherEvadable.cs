﻿namespace O9K.Evader.Abilities.Heroes.ShadowDemon.SoulCatcher;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class SoulCatcherEvadable : LinearAreaOfEffectEvadable, IModifierCounter
{
    public SoulCatcherEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.PhaseShift);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.SleightOfFist);

        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
        this.ModifierCounters.Add(Abilities.MantaStyle);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}