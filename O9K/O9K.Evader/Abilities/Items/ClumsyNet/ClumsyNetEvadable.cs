﻿namespace O9K.Evader.Abilities.Items.ClumsyNet;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class ClumsyNetEvadable : ProjectileEvadable, IModifierCounter
{
    public ClumsyNetEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Counters.UnionWith(Abilities.VsDisableProjectile);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.Invisibility);
        this.Counters.UnionWith(Abilities.SlowHeal);

        this.Counters.ExceptWith(Abilities.MagicImmunity);
        this.Counters.Remove(Abilities.ShadowDance);
        this.Counters.Remove(Abilities.SpikedCarapace);
        this.Counters.Remove(Abilities.Refraction);
        this.Counters.Remove(Abilities.EulsScepterOfDivinity);
        this.Counters.Remove(Abilities.WindWaker);
        this.Counters.Remove(Abilities.Stormcrafter);
        this.Counters.Remove(Abilities.MantaStyle);
        this.Counters.Remove(Abilities.Enrage);

        this.ModifierCounters.Add(Abilities.MantaStyle);
        this.ModifierCounters.Add(Abilities.Enrage);
        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
        this.ModifierCounters.Add(Abilities.PressTheAttack);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}