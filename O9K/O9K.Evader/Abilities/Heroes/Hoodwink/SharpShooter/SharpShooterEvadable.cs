namespace O9K.Evader.Abilities.Heroes.Hoodwink.SharpShooter;

using System;

using Base.Evadable;
using Base.Evadable.Components;

using Core.Entities.Abilities.Base;
using Core.Logger;

using Divine.Entity.Entities.Units;
using Divine.Game;
using Divine.Particle.Particles;
using Divine.Update;

using Metadata;

using Pathfinder.Obstacles.Abilities.LinearProjectile;

using Windranger.Powershot;

internal sealed class SharpShooterEvadable :  LinearProjectileEvadable, IUnit, IParticle
{
    public SharpShooterEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Blinks.UnionWith(Abilities.Blink);

        this.Disables.UnionWith(Abilities.Disable);
        this.Disables.UnionWith(Abilities.PhysDisable);

        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.Spoink);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.MagicShield);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.Add(Abilities.Armlet);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.UnionWith(Abilities.VsProjectile);

        this.Counters.Remove(Abilities.DarkPact);
        this.Counters.Remove(Abilities.ShadowDance);
        this.Counters.Remove(Abilities.ShadowRealm);
        this.Counters.ExceptWith(Abilities.MagicImmunity);

        this.ModifierCounters.UnionWith(Abilities.AllyStrongPurge);
        this.ModifierCounters.UnionWith(Abilities.Invulnerability);
        this.ModifierCounters.UnionWith(Abilities.StrongPhysShield);
    }

    public void AddParticle(Particle particle, string name)
    {
        if (!this.Owner.IsVisible)
        {
            return;
        }

        var time = GameManager.RawGameTime - GameManager.Ping / 2000;

        UpdateManager.BeginInvoke(
            100,
            () =>
            {
                try
                {
                    var startPosition = this.Owner.Position;
                    var direction = this.Owner.InFront(this.Ability.Range);

                    var obstacle = new PowershotObstacle(this, startPosition, direction)
                    {
                        EndCastTime = time,
                        EndObstacleTime =
                            time + this.RangedAbility.Range / this.RangedAbility.Speed,
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

        var time = GameManager.RawGameTime - GameManager.Ping / 2000;

        var obstacle = new LinearProjectileUnitObstacle(this, unit)
        {
            EndCastTime = time,
            EndObstacleTime = time + this.RangedAbility.Range / this.RangedAbility.Speed,
            ActivationDelay = 0,
        };

        this.Pathfinder.AddObstacle(obstacle);
    }
}