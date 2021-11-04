﻿namespace O9K.Evader.Abilities.Heroes.Meepo.Earthbind;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class EarthbindEvadable : LinearProjectileEvadable, IModifierCounter
{
    public EarthbindEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Disables.UnionWith(Abilities.Disable);

        this.Counters.Add(Abilities.BallLightning);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.SlowHeal);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);

        this.Counters.Remove(Abilities.Enrage);

        this.ModifierCounters.Add(Abilities.MantaStyle);
        this.ModifierCounters.UnionWith(Abilities.AllyPurge);
        this.ModifierCounters.Add(Abilities.PressTheAttack);
        this.ModifierCounters.Add(Abilities.Enrage);
        this.ModifierCounters.Add(Abilities.ChemicalRage);
        this.ModifierCounters.UnionWith(Abilities.Invulnerability);
    }

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }
}