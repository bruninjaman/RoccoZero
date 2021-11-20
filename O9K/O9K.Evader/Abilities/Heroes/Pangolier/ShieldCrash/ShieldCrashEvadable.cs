﻿namespace O9K.Evader.Abilities.Heroes.Pangolier.ShieldCrash;

using System;

using Base.Evadable;
using Base.Evadable.Components;

using Core.Entities.Abilities.Base;
using Core.Extensions;
using Core.Logger;
using Divine.Game;
using Divine.Update;
using Divine.Particle.Particles;

using Metadata;

using Pathfinder.Obstacles.Abilities.AreaOfEffect;

internal sealed class ShieldCrashEvadable : AreaOfEffectEvadable, IParticle
{
    public ShieldCrashEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.Mischief);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.StrongMagicShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);

        this.Counters.Remove(Abilities.DarkPact);
        this.Counters.Remove(Abilities.ShadowDance);
        this.Counters.Remove(Abilities.ShadowRealm);
    }

    public void AddParticle(Particle particle, string name)
    {
        var time = GameManager.RawGameTime - (GameManager.Ping / 2000);
        var startPosition = this.Owner.Position;

        UpdateManager.BeginInvoke(
            50,
            () =>
                {
                    try
                    {
                        if (!particle.IsValid)
                        {
                            return;
                        }

                        var position = startPosition.Extend2D(particle.GetControlPoint(0), this.Ability.CastRange);

                        var obstacle = new AreaOfEffectObstacle(this, position)
                        {
                            EndCastTime = time,
                            EndObstacleTime = time + this.Ability.ActivationDelay
                        };

                        this.Pathfinder.AddObstacle(obstacle);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                });
    }

    public override void PhaseCancel()
    {
    }

    public override void PhaseStart()
    {
    }
}