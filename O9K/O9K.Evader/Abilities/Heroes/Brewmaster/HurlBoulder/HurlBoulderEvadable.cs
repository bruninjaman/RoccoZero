﻿namespace O9K.Evader.Abilities.Heroes.Brewmaster.HurlBoulder;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class HurlBoulderEvadable : TargetableProjectileEvadable, IModifierCounter
{
    public HurlBoulderEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Counters.Add(Abilities.Meld);
        this.Counters.Add(Abilities.Shukuchi);
        this.Counters.UnionWith(Abilities.VsDisableProjectile);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.Invulnerability);
        this.Counters.UnionWith(Abilities.StrongPhysShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.UnionWith(Abilities.Invisibility);
        this.Counters.UnionWith(Abilities.SlowHeal);

        this.ModifierCounters.UnionWith(Abilities.AllyStrongPurge);
        this.ModifierCounters.UnionWith(Abilities.Invulnerability);
        this.ModifierCounters.UnionWith(Abilities.StrongPhysShield);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}