﻿namespace O9K.Evader.Abilities.Heroes.AncientApparition.IceBlast;

using Base.Evadable;

using Divine.Numerics;

using O9K.Core.Geometry;

using Pathfinder.Obstacles.Abilities.AreaOfEffect;

internal class IceBlastObstacle : AreaOfEffectObstacle
{
    public IceBlastObstacle(EvadableAbility ability, Vector3 position, float radius)
        : base(ability)
    {
        const int RadiusIncrease = 50;

        this.Position = position;
        this.Radius = radius + RadiusIncrease;
        this.Polygon = new Polygon.Circle(this.Position, this.Radius);
        this.NavMeshObstacles = this.Pathfinder.AddNavMeshObstacle(this.Position, this.Radius);
        this.NavMeshId = 1; // hack
    }
}