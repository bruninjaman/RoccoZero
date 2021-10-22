﻿namespace O9K.Evader.Abilities.Heroes.Windranger.Powershot;

using System;

using Base.Evadable;

using Core.Entities.Units;
using Core.Extensions;

using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;

using O9K.Core.Geometry;

using Pathfinder.Obstacles.Abilities.LinearProjectile;

internal class PowershotObstacle : LinearProjectileObstacle
{
    private float channelTime;

    public PowershotObstacle(LinearProjectileEvadable ability, Vector3 startPosition, Vector3 direction)
        : base(ability, startPosition, direction)
    {
    }

    public override void Draw()
    {
        if (this.NavMeshId == null)
        {
            return;
        }

        this.Drawer.DrawArcRectangle(this.Position, this.EndPosition, this.Radius);

        var time = (GameManager.RawGameTime - this.EndCastTime - this.GetChannelTime()) + (this.Radius / 2 / this.Speed);
        if (time < 0)
        {
            return;
        }

        this.Drawer.DrawCircle(this.Position, this.Radius);
        this.Drawer.UpdateCirclePosition(this.Position.Extend2D(this.EndPosition, time * this.Speed));
    }

    public override float GetDisableTime(Unit9 enemy)
    {
        return this.EndCastTime - GameManager.RawGameTime;
    }

    public override float GetEvadeTime(Unit9 ally, bool blink)
    {
        var distance = Math.Max(ally.Distance(this.Position) - this.Radius, 0);
        return (this.EndCastTime + this.GetChannelTime() + (distance / this.Speed)) - GameManager.RawGameTime;
    }

    public override void Update()
    {
        if (this.NavMeshId == null /*&& !this.Caster.IsRotating*/)
        {
            this.EndPosition = this.Caster.InFront(this.Range);
            this.Polygon = new Polygon9.Rectangle(this.Position, this.EndPosition, this.Radius);
            this.NavMeshObstacles = this.Pathfinder.AddNavMeshObstacle(this.Position, this.EndPosition, this.Radius);
            this.NavMeshId = 1; //hack
        }
        else if (this.NavMeshId != null)
        {
            ////todo check activation delay of all obstacles
            var time = GameManager.RawGameTime - this.EndCastTime - this.GetChannelTime() /*- 0.35f*/;
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

    private float GetChannelTime()
    {
        if (!this.Caster.IsVisible || this.Caster.IsChanneling)
        {
            return this.ActivationDelay;
        }

        if (this.channelTime <= 0)
        {
            this.channelTime = Math.Min(GameManager.RawGameTime - this.EndCastTime, this.ActivationDelay);
            this.EndObstacleTime -= this.ActivationDelay - this.channelTime;
        }

        return this.channelTime;
    }
}