﻿namespace O9K.Evader.Abilities.Heroes.Beastmaster.PrimalRoar;

using Base;
using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Modifier.Modifiers;

using Metadata;

using Pathfinder.Obstacles.Modifiers;

internal sealed class PrimalRoarEvadable : TargetableEvadable, IModifierCounter
{
    public PrimalRoarEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Disables.UnionWith(Abilities.Disable);

        this.Counters.Add(Abilities.Counterspell);
        this.Counters.Add(Abilities.LinkensSphere);
        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.Mischief);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.Invulnerability);
        this.Counters.Add(Abilities.LotusOrb);
        this.Counters.Add(Abilities.Spoink);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.UnionWith(Abilities.StrongPhysShield);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.UnionWith(Abilities.Invisibility);
        this.Counters.UnionWith(Abilities.SlowHeal);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);
        this.Counters.Add(Abilities.Bulwark);

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

    //protected override void AddObstacle()
    //{
    //    //todo aoe obstacle ?
    //    var obstacle = new TargetableObstacle(this, this.ActiveAbility.Radius)
    //    {
    //        EndCastTime = this.EndCastTime,
    //        EndObstacleTime = this.EndCastTime + this.Ability.ActivationDelay
    //    };

    //    this.Pathfinder.AddObstacle(obstacle);
    //}
}