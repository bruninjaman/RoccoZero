﻿namespace O9K.Evader.Abilities.Heroes.LegionCommander.PressTheAttack;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class PressTheAttackEvadable : ModifierCounterEvadable
{
    public PressTheAttackEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierCounters.UnionWith(Abilities.PhysShield);

        this.ModifierDisables.UnionWith(Abilities.EnemyPurge);
        this.ModifierDisables.UnionWith(Abilities.PhysDisable);
    }

    public override bool ModifierEnemyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierEnemyObstacle(this, modifier, modifierOwner, 400);
        this.Pathfinder.AddObstacle(obstacle);
    }
}