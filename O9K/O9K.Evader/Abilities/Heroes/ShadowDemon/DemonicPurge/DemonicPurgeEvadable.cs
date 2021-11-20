﻿namespace O9K.Evader.Abilities.Heroes.ShadowDemon.DemonicPurge;

using Base;
using Base.Evadable;
using Base.Usable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles;
using Pathfinder.Obstacles.Modifiers;

internal sealed class DemonicPurgeEvadable : TargetableEvadable, IModifierCounter
{
    public DemonicPurgeEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Disables.UnionWith(Abilities.Disable);

        this.Counters.Add(Abilities.Counterspell);
        this.Counters.Add(Abilities.LinkensSphere);
        this.Counters.Add(Abilities.LotusOrb);

        this.ModifierCounters.Add(Abilities.BallLightning);
        this.ModifierCounters.Add(Abilities.MantaStyle);
        this.ModifierCounters.UnionWith(Abilities.Shield);
        this.ModifierCounters.UnionWith(Abilities.AllyStrongPurge);
        this.ModifierCounters.UnionWith(Abilities.StrongMagicShield);
        this.ModifierCounters.Add(Abilities.BladeMail);
        this.ModifierCounters.Add(Abilities.ArcanistArmor);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }

    public override bool IgnoreRemainingTime(IObstacle obstacle, UsableAbility usableAbility)
    {
        return false;
    }
}