﻿namespace O9K.Evader.Abilities.Heroes.Riki.SmokeScreen;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class SmokeScreenEvadable : LinearAreaOfEffectEvadable, IModifierCounter
{
    public SmokeScreenEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Disables.UnionWith(Abilities.Disable);

        this.Counters.Add(Abilities.BallLightning);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.SlowHeal);
        this.Counters.UnionWith(Abilities.Invisibility);
        this.Counters.Add(Abilities.Spoink);

        //todo modifier blink?

        this.Counters.Remove(Abilities.Enrage);
        this.Counters.Remove(Abilities.EulsScepterOfDivinity);
        this.Counters.Remove(Abilities.WindWaker);
        this.Counters.Remove(Abilities.Stormcrafter);

        this.ModifierCounters.Add(Abilities.HurricanePike);
        this.ModifierCounters.Add(Abilities.PsychicHeadband);
        this.ModifierCounters.UnionWith(Abilities.StrongShield);
        this.ModifierCounters.Remove(Abilities.EulsScepterOfDivinity);
        this.ModifierCounters.Remove(Abilities.WindWaker);
        this.ModifierCounters.Remove(Abilities.Stormcrafter);
        this.ModifierCounters.Remove(Abilities.Enrage);

        this.ModifierDisables.UnionWith(Abilities.PhysDisable);
        this.ModifierDisables.UnionWith(Abilities.Invulnerability);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner)
        {
            IgnoreModifierRemainingTime = true
        };

        this.Pathfinder.AddObstacle(obstacle);
    }
}