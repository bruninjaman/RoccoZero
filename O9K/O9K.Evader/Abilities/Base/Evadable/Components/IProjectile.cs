namespace O9K.Evader.Abilities.Base.Evadable.Components;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Projectile.Projectiles;

internal interface IProjectile
{
    ActiveAbility ActiveAbility { get; }

    void AddProjectile(TrackingProjectile projectile, Unit9 target);
}