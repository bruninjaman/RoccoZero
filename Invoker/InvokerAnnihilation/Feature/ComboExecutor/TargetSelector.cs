using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;

namespace InvokerAnnihilation.Feature.ComboExecutor;

public static class TargetSelector
{
    public static Hero? GetTarget()
    {
        var mousePosition = GameManager.MousePosition;
        var me = EntityManager.LocalHero!;
        var entity = EntityManager.GetEntities<Hero>()
            .Where(x => x.IsValid && x.IsAlive && x.IsEnemy(me) && x.IsVisible && x.IsInRange(mousePosition, 350))
            .OrderBy(x => x.Distance2D(mousePosition))
            .FirstOrDefault();

        return entity;
    }
}