using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Projectile.Projectiles;

namespace Farmling.Global;

public static class GlobalStatsService
{
    public static Dictionary<string, List<float>> ProjectileExtraRangeHistory { get; set; } = new Dictionary<string, List<float>>();

    public static void SaveProjectileToStatistics(TrackingProjectile projectile)
    {
        var source = projectile.Source as Unit;
        var extraRange = (projectile.Position.Distance2D(source!.Position) - source.ProjectileCollisionSize - source.HullRadius);
        var key = $"{source.Name}-{source.NetworkActivity}";
        if (ProjectileExtraRangeHistory.TryGetValue(key, out var list))
        {
            if (!list.Contains(extraRange))
            {
                list.Add(extraRange);
            }
        }
        else
        {
            ProjectileExtraRangeHistory.Add(key, new List<float> {extraRange});
        }
    }

    public static float PredictProjectileExtraRange(Unit source)
    {
        var key = $"{source.Name}-{source.NetworkActivity}";
        if (ProjectileExtraRangeHistory.TryGetValue(key, out var list))
        {
            return list.Average();
        }

        return 0;
    }
}
