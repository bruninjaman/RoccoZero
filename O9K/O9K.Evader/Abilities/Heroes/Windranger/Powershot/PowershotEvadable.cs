﻿namespace O9K.Evader.Abilities.Heroes.Windranger.Powershot;

using System;

using Base.Evadable;
using Base.Evadable.Components;

using Core.Entities.Abilities.Base;
using Core.Logger;
using Divine.Game;
using Divine.Update;
using Divine.Particle.Particles;
using Divine.Entity.Entities.Units;

using Metadata;

using Pathfinder.Obstacles.Abilities.LinearProjectile;

internal sealed class PowershotEvadable : LinearProjectileEvadable, IParticle, IUnit
{
    public PowershotEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.Spoink);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.Shield);
        this.Counters.UnionWith(Abilities.MagicShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
    }

    public void AddParticle(Particle particle, string name)
    {
        if (!this.Owner.IsVisible)
        {
            return;
        }

        var time = GameManager.RawGameTime - (GameManager.Ping / 2000);

        UpdateManager.BeginInvoke(
            100,
            () =>
                {
                    try
                    {
                        var startPosition = this.Owner.Position;
                        var direction = this.Owner.InFront(500);

                        var obstacle = new PowershotObstacle(this, startPosition, direction)
                        {
                            EndCastTime = time,
                            EndObstacleTime =
                                time + this.Ability.ActivationDelay + (this.RangedAbility.Range / this.RangedAbility.Speed)
                        };

                        this.Pathfinder.AddObstacle(obstacle);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                });
    }

    public void AddUnit(Unit unit)
    {
        if (this.Owner.IsVisible)
        {
            return;
        }

        var time = GameManager.RawGameTime - (GameManager.Ping / 2000);

        var obstacle = new LinearProjectileUnitObstacle(this, unit)
        {
            EndCastTime = time,
            EndObstacleTime = time + (this.RangedAbility.Range / this.RangedAbility.Speed),
            ActivationDelay = 0
        };

        this.Pathfinder.AddObstacle(obstacle);
    }

    protected override void AddObstacle()
    {
    }
}