﻿namespace O9K.Evader.Abilities.Heroes.Leshrac.PulseNova;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class PulseNovaEvadable : ModifierCounterEvadable
{
    public PulseNovaEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierCounters.UnionWith(Abilities.StrongMagicShield);
    }

    public override bool ModifierEnemyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierEnemyObstacle(this, modifier, modifierOwner, this.ActiveAbility.Radius);
        this.Pathfinder.AddObstacle(obstacle);
    }
}