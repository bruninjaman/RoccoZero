using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;

using System.Linq;

using static TinkerEW.Data.TargetSelector;

namespace TinkerEW
{
    internal sealed class TargetSelector
    {
        private Hero? Target;
        private readonly TargetSelectorModes TargetSelectorMode;
        private readonly Menu Menu;

        public TargetSelector(Menu menu)
        {
            Menu = menu;
            if (Menu.ComboTargetSelectorMode == "Nearest To Cursor")
            {
                TargetSelectorMode = TargetSelectorModes.NearestToCursor;
            }
            else
            {
                TargetSelectorMode = TargetSelectorModes.InRadiusFromCursor;
            }
        }

        public Hero? GetTarget()
        {
            var localHero = EntityManager.LocalHero;
            var cursorPos = GameManager.MousePosition;
            if (Target == null)
            {
                if (TargetSelectorMode == TargetSelectorModes.NearestToCursor)
                {
                    Target = EntityManager.GetEntities<Hero>()
                        .OrderBy(x => x.Distance2D(cursorPos))
                        .FirstOrDefault(x => localHero != null
                        && !x.IsAlly(localHero)
                        && !x.IsIllusion
                        && x.IsVisible
                        && x.IsAlive);
                    return Target;
                }
                else
                {
                    Target = EntityManager.GetEntities<Hero>()
                        .FirstOrDefault(x => x.Distance2D(cursorPos) <= Menu.ComboTargetSelectorRadius.Value
                        && localHero != null
                        && !x.IsAlly(localHero)
                        && !x.IsIllusion
                        && x.IsVisible
                        && x.IsAlive);
                    return Target;
                }
            }
            else
            {
                if (Target.IsVisible && Target.IsAlive)
                {
                    return Target;
                }
                else
                {
                    Target = null;
                    return Target;
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
