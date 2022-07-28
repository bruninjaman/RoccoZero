using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Particle;
using Divine.Projectile;
using Farmling.Interfaces;
using Farmling.LoggerService;

namespace Farmling.Services;

public class ProjectileTrackManager : IProjectileTrackManager
{
    public ProjectileTrackManager()
    {
        // ProjectileManager.LinearProjectileAdded += args =>
        // {
        //     Logger.Log($"LinearProjectile: {args.Projectile.Handle} {args.Projectile.StartPosition}");
        // };
        var dict = new Dictionary<string, List<string>>();
        ProjectileManager.TrackingProjectileAdded += args =>
        {
            if (args.Projectile.IsAttack && !args.Projectile.IsEvaded)
            {
                var source = (args.Projectile.Source as Unit)!;
                var animation = args.Projectile.Source.AnimationName;
                var dif = (args.Projectile.Position.Distance2D(source.Position) - source.HullRadius).ToString("F");
                if (dict.TryGetValue(animation, out var list))
                {
                    if (!list.Contains(dif))
                    {
                        list.Add(dif);
                        Logger.Log("===============================================");
                        Logger.Log($"New Projectile: {animation} Dist: {dif} | {list.Count}");
                        Logger.Log("===============================================");
                    }
                }
                else
                {
                    dict.Add(animation, new List<string> {dif});
                }

                Notify?.Invoke(args.Projectile, true);
            }
        };
        ProjectileManager.TrackingProjectileDestroy += args =>
        {
            if (args.Projectile.IsAttack && !args.Projectile.IsEvaded)
            {
                Notify?.Invoke(args.Projectile, false);
                Logger.Log($"Destroy: {args.Projectile.Handle} ");
            }
        };

        ParticleManager.ParticleAdded += args =>
        {
            if (args.Particle.Name == "particles/generic_gameplay/generic_hit_blood.vpcf") Logger.Log($"ParticleAdded: {args.Particle.Name}");
        };
    }

    public event IProjectileTrackManager.AttackProjectileEvent? Notify;
}
