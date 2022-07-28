using Divine.Projectile.Projectiles;

namespace Farmling.Interfaces;

public interface IProjectileTrackManager
{
    delegate void AttackProjectileEvent(TrackingProjectile projectile, bool isCreated);

    event AttackProjectileEvent? Notify;
}
