﻿namespace O9K.Evader.Abilities.Heroes.Visage.StoneForm;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class StoneFormEvadable : AreaOfEffectEvadable, IModifierCounter
{
    public StoneFormEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.Spoink);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.Invulnerability);
        this.Counters.Add(Abilities.HurricanePike);
        this.Counters.Add(Abilities.Dispose);
        this.Counters.Add(Abilities.PsychicHeadband); //todo test
        this.Counters.UnionWith(Abilities.MagicShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);

        this.ModifierCounters.UnionWith(Abilities.AllyStrongPurge);
        this.ModifierCounters.UnionWith(Abilities.Invulnerability);
        this.ModifierCounters.UnionWith(Abilities.StrongMagicShield);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}