﻿namespace O9K.Evader.Abilities.Heroes.Enigma.Malefice;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class MaleficeEvadable : TargetableEvadable, IModifierCounter
{
    public MaleficeEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Disables.UnionWith(Abilities.Disable);

        this.Counters.Add(Abilities.Counterspell);
        this.Counters.Add(Abilities.LinkensSphere);
        this.Counters.UnionWith(Abilities.Shield);
        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.BallLightning);

        this.ModifierCounters.Add(Abilities.MantaStyle);
        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
        this.ModifierCounters.Add(Abilities.PressTheAttack);
        this.ModifierCounters.UnionWith(Abilities.MagicShield);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}