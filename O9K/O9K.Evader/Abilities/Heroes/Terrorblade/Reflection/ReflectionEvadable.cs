﻿namespace O9K.Evader.Abilities.Heroes.Terrorblade.Reflection;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class ReflectionEvadable : AreaOfEffectEvadable, IModifierCounter
{
    public ReflectionEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.PhaseShift);
        this.Counters.Add(Abilities.EulsScepterOfDivinity);
        this.Counters.Add(Abilities.WindWaker);
        this.Counters.Add(Abilities.Stormcrafter);
        this.Counters.Add(Abilities.SleightOfFist);

        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}