using Divine.Particle;
using Divine.Projectile;
using Farmling.Interfaces;
using Farmling.LoggerService;

namespace Farmling.Services;

public class ProjectileTrackManager : IProjectileTrackManager
{
    public ProjectileTrackManager()
    {
        ProjectileManager.TrackingProjectileAdded += args =>
        {
            if (args.Projectile.IsAttack && !args.Projectile.IsEvaded)
            {
                Notify?.Invoke(args.Projectile, true);
            }
        };
        ProjectileManager.TrackingProjectileDestroy += args =>
        {
            if (args.Projectile.IsAttack && !args.Projectile.IsEvaded)
            {
                Notify?.Invoke(args.Projectile, false);
            }
        };

        ParticleManager.ParticleAdded += args =>
        {
            if (args.Particle.Name == "particles/generic_gameplay/generic_hit_blood.vpcf") Logger.Log($"ParticleAdded: {args.Particle.Name}");
        };
    }

    public event IProjectileTrackManager.AttackProjectileEvent? Notify;
}
