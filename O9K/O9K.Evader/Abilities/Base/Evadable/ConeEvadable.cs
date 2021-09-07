﻿namespace O9K.Evader.Abilities.Base.Evadable;

using Core.Entities.Abilities.Base;

using Metadata;

using Pathfinder.Obstacles.Abilities.Cone;

internal class ConeEvadable : EvadableAbility
{
    public ConeEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.ConeAbility = (ConeAbility)ability;
    }

    public ConeAbility ConeAbility { get; }

    protected override void AddObstacle()
    {
        var obstacle = new ConeObstacle(this, this.Owner.Position)
        {
            EndCastTime = this.EndCastTime,
            EndObstacleTime = this.EndCastTime + this.Ability.ActivationDelay,
        };

        this.Pathfinder.AddObstacle(obstacle);
    }
}