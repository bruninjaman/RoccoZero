﻿namespace O9K.Evader.Abilities.Heroes.Zeus.LightningBolt;

using System.Collections.Generic;
using System.Linq;

using Base.Evadable;

using Core.Entities.Abilities.Base;

using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using Pathfinder.Obstacles.Abilities.LinearAreaOfEffect;
using Pathfinder.Obstacles.Abilities.Targetable;

internal sealed class LightningBoltEvadable : LinearAreaOfEffectEvadable
{
    private readonly HashSet<AbilityId> targetCounters = new HashSet<AbilityId>();

    public LightningBoltEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.Counterspell);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.Shield);
        this.Counters.UnionWith(Abilities.MagicShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);

        this.targetCounters.Add(Abilities.Counterspell);
        this.targetCounters.Add(Abilities.LinkensSphere);
        this.targetCounters.Add(Abilities.LotusOrb);
    }

    protected override IEnumerable<AbilityId> AllCounters
    {
        get
        {
            return base.AllCounters.Concat(this.targetCounters);
        }
    }

    public override void PhaseCancel()
    {
        base.PhaseCancel();

        this.Pathfinder.CancelObstacle(this.Ability.Handle);
    }

    protected override void AddObstacle()
    {
        var obstacle = new LinearAreaOfEffectObstacle(this, this.Owner.Position)
        {
            EndCastTime = this.EndCastTime,
            EndObstacleTime = this.EndCastTime + this.Ability.ActivationDelay,
        };

        this.Pathfinder.AddObstacle(obstacle);

        var targetableObstacle = new TargetableObstacle(this)
        {
            Id = obstacle.Id,
            EndCastTime = this.EndCastTime,
            EndObstacleTime = this.EndCastTime,
            Counters = this.targetCounters.ToArray()
        };

        this.Pathfinder.AddObstacle(targetableObstacle);
    }
}