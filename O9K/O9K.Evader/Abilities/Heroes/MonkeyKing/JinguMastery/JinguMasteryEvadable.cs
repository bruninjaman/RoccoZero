﻿namespace O9K.Evader.Abilities.Heroes.MonkeyKing.JinguMastery;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class JinguMasteryEvadable : ModifierCounterEvadable
{
    public JinguMasteryEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierDisables.UnionWith(Abilities.EnemyPurge);
        this.ModifierDisables.UnionWith(Abilities.PhysDisable);
    }

    public override bool ModifierEnemyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierEnemyObstacle(this, modifier, modifierOwner, 600);
        this.Pathfinder.AddObstacle(obstacle);
    }
}