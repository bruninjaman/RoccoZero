﻿namespace O9K.Evader.Abilities.Heroes.Huskar.BurningSpear;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class BurningSpearEvadable : ModifierCounterEvadable
{
    public BurningSpearEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierCounters.UnionWith(Abilities.MagicShield);
    }

    public override bool ModifierAllyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierStackAllyObstacle(this, modifier, modifierOwner, 5);
        this.Pathfinder.AddObstacle(obstacle);
    }
}