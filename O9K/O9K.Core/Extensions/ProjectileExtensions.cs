namespace O9K.Core.Extensions
{
    using Divine;

    public static class ProjectileExtensions
    {
        public static bool IsAutoAttackProjectile(this TrackingProjectile projectile)
        {
            return projectile.IsAttack;
        }
    }
}