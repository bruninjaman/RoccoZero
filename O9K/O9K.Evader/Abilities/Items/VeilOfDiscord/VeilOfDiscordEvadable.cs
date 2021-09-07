﻿namespace O9K.Evader.Abilities.Items.VeilOfDiscord;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class VeilOfDiscordEvadable : ModifierCounterEvadable
{
    public VeilOfDiscordEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
        this.ModifierCounters.UnionWith(Abilities.StrongMagicShield);
        this.ModifierCounters.Remove(Abilities.EulsScepterOfDivinity);
        this.ModifierCounters.Remove(Abilities.WindWaker);
        this.ModifierCounters.Remove(Abilities.Stormcrafter);
    }

    public override bool ModifierAllyCounter { get; } = true;

    public override void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}