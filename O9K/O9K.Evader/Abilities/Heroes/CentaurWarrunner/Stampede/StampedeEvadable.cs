﻿namespace O9K.Evader.Abilities.Heroes.CentaurWarrunner.Stampede;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class StampedeEvadable : ModifierCounterEvadable
{
    public StampedeEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierDisables.UnionWith(Abilities.Root);
        this.ModifierDisables.UnionWith(Abilities.Invulnerability);
    }

    public override bool ModifierEnemyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierEnemyObstacle(this, modifier, modifierOwner, 500);
        this.Pathfinder.AddObstacle(obstacle);
    }
}