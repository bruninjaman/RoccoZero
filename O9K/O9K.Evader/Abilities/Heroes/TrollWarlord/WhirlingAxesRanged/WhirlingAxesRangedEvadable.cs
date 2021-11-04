﻿namespace O9K.Evader.Abilities.Heroes.TrollWarlord.WhirlingAxesRanged;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class WhirlingAxesRangedEvadable : ConeProjectileEvadable, IModifierCounter
{
    public WhirlingAxesRangedEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.Shield);
        this.Counters.UnionWith(Abilities.MagicShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);

        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}