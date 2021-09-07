﻿namespace O9K.Evader.Abilities.Heroes.BountyHunter.Track;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class TrackEvadable : TargetableEvadable, IModifierCounter
{
    public TrackEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.Counterspell);

        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
        this.ModifierCounters.Remove(Abilities.EulsScepterOfDivinity);
        this.ModifierCounters.Remove(Abilities.WindWaker);
        this.ModifierCounters.Remove(Abilities.Stormcrafter);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}