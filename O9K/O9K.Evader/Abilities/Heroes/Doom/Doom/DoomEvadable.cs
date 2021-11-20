﻿namespace O9K.Evader.Abilities.Heroes.Doom.Doom;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class DoomEvadable : TargetableEvadable, IModifierCounter
{
    public DoomEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Disables.UnionWith(Abilities.Disable);
        this.Disables.UnionWith(Abilities.Invulnerability);

        this.Counters.Add(Abilities.Counterspell);
        this.Counters.Add(Abilities.LinkensSphere);
        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.Invulnerability);
        this.Counters.Add(Abilities.LotusOrb);
        this.Counters.UnionWith(Abilities.StrongMagicShield);
        this.Counters.UnionWith(Abilities.SlowHeal);
        this.Counters.UnionWith(Abilities.Invisibility);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);
        this.Counters.Add(Abilities.Spoink);

        this.ModifierCounters.UnionWith(Abilities.StrongMagicShield);
        this.ModifierCounters.UnionWith(Abilities.Invulnerability);
        this.ModifierCounters.Add(Abilities.Armlet);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}