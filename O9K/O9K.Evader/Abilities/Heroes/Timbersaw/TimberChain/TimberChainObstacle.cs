﻿namespace O9K.Evader.Abilities.Heroes.Timbersaw.TimberChain;

using System.Linq;

using Base.Evadable;

using Core.Entities.Abilities.Heroes.Timbersaw;
using Core.Extensions;
using Core.Managers.Entity;

using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;

using O9K.Core.Geometry;

using Pathfinder.Obstacles.Abilities.LinearProjectile;

internal class TimberChainObstacle : LinearProjectileObstacle
{
    private readonly TimberChain timberChain;

    public TimberChainObstacle(LinearProjectileEvadable ability, Vector3 startPosition)
        : base(ability, startPosition)
    {
        this.timberChain = (TimberChain)ability.RangedAbility;
    }

    public override void Update()
    {
        if (this.NavMeshId == null /*&& !this.Caster.IsRotating*/)
        {
            var chain = new Polygon9.Rectangle(this.Position, this.Caster.InFront(this.Range), this.timberChain.ChainRadius);
            var tree = EntityManager9.Trees.Where(x => x.Distance2D(this.Position) < this.Range && chain.IsInside(x.Position))
                .OrderBy(x => x.Distance2D(this.Position))
                .FirstOrDefault();

            if (tree == null)
            {
                this.EndObstacleTime = 0;
                this.IsUpdated = true;
                return;
            }

            this.EndPosition = tree.Position;
            this.ActivationDelay = this.Position.Distance2D(this.EndPosition) / this.Speed;
            this.EndObstacleTime += this.ActivationDelay * 2;
            this.Polygon = new Polygon9.Rectangle(this.Position, this.EndPosition, this.Radius);
            this.NavMeshObstacles = this.Pathfinder.AddNavMeshObstacle(this.Position, this.EndPosition, this.Radius);
            this.NavMeshId = 1; //hack
        }
        else if (this.NavMeshId != null)
        {
            ////todo check activation delay of all obstacles
            var time = GameManager.RawGameTime - this.EndCastTime - this.ActivationDelay /*- 0.35f*/;
            if (time < 0)
            {
                return;
            }

            var currentPosition = this.Position.Extend2D(this.EndPosition, time * this.Speed);

            //this.Pathfinder.NavMesh.UpdateObstacle(this.NavMeshId.Value, currentPosition, this.Radius);
            var rectangle = (Polygon9.Rectangle)this.Polygon;
            rectangle.Start = currentPosition.ToVector2();
            rectangle.UpdatePolygon();
        }
    }
}