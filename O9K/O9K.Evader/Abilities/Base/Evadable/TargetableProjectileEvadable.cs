﻿namespace O9K.Evader.Abilities.Base.Evadable;

using Components;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;
using Divine.Game;
using Divine.Projectile.Projectiles;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using Pathfinder.Obstacles;
using Pathfinder.Obstacles.Abilities;
using Pathfinder.Obstacles.Abilities.Projectile;

using Usable;

internal abstract class TargetableProjectileEvadable : EvadableAbility, IProjectile
{
    protected TargetableProjectileObstacle TargetableObstacle;

    protected TargetableProjectileEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
    }

    public override bool CanBeDodged { get; } = false;

    public virtual bool IsDisjointable { get; } = true;

    public virtual void AddProjectile(TrackingProjectile projectile, Unit9 target)
    {
        if (this.TargetableObstacle != null && this.EndCastTime + 0.5f > GameManager.RawGameTime)
        {
            this.TargetableObstacle.AddProjectile(projectile, target);
        }
        else
        {
            var projectileObstacle = new ProjectileObstacle(this, projectile, target);
            this.Pathfinder.AddObstacle(projectileObstacle);
        }
    }

    public override bool IgnoreRemainingTime(IObstacle obstacle, UsableAbility usableAbility)
    {
        if (usableAbility?.Ability.Id == AbilityId.ember_spirit_sleight_of_fist)
        {
            if (obstacle is AbilityObstacle ability)
            {
                //todo move ignore time to usable abilities ?
                return GameManager.RawGameTime > ability.EndCastTime + 0.05f;
            }
        }

        return base.IgnoreRemainingTime(obstacle, usableAbility);
    }

    protected override void AddObstacle()
    {
        this.TargetableObstacle = new TargetableProjectileObstacle(this);
        this.Pathfinder.AddObstacle(this.TargetableObstacle);
    }
}