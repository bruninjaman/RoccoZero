﻿namespace O9K.Evader.Abilities.Items.GhostScepter;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class GhostScepterEvadable : ModifierCounterEvadable
{
    public GhostScepterEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierDisables.UnionWith(Abilities.EnemyPurge);
    }

    public override bool ModifierEnemyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierEnemyObstacle(this, modifier, modifierOwner, 600);
        this.Pathfinder.AddObstacle(obstacle);
    }
}