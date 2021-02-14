using System.Linq;

using Divine.SDK.Extensions;

namespace Divine.SkywrathMage.TargetSelector.Selector
{
    internal sealed class NearMouse : ISelector
    {
        public Hero GetTarget()
        {
            var localHero = EntityManager.LocalHero;
            var mousePosition = GameManager.MousePosition;

            return EntityManager
                .GetEntities<Hero>()
                .Where(x => !x.IsAlly(localHero) && !x.IsIllusion && x.IsVisible && x.IsAlive && x.Distance2D(mousePosition) <= 800)
                .OrderBy(x => x.Distance2D(mousePosition))
                .FirstOrDefault();
        }
    }
}