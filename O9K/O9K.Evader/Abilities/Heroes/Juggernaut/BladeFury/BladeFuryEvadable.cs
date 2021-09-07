﻿namespace O9K.Evader.Abilities.Heroes.Juggernaut.BladeFury;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class BladeFuryEvadable : ModifierCounterEvadable
{
    public BladeFuryEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierCounters.UnionWith(Abilities.Shield);
        this.ModifierCounters.Add(Abilities.EulsScepterOfDivinity);
        this.ModifierCounters.Add(Abilities.WindWaker);
        this.ModifierCounters.Add(Abilities.Stormcrafter);
        this.ModifierCounters.UnionWith(Abilities.MagicShield);

        this.ModifierCounters.Remove(Abilities.SpikedCarapace);

        this.ModifierDisables.UnionWith(Abilities.Root);
    }

    public override bool ModifierEnemyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierEnemyObstacle(this, modifier, modifierOwner, this.ActiveAbility.Radius);
        this.Pathfinder.AddObstacle(obstacle);
    }
}