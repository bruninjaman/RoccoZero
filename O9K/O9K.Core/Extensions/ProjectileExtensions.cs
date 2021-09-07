namespace O9K.Core.Extensions;

using Divine.Projectile.Projectiles;

public static class ProjectileExtensions
{
    public static bool IsAutoAttackProjectile(this TrackingProjectile projectile)
    {
        return projectile.IsAttack;
    }
}