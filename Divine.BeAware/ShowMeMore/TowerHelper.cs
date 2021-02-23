using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.SDK.Extensions;

namespace Divine.BeAware.ShowMeMore
{
    internal sealed class TowerHelper
    {
        private readonly TowerHelperMenu TowerHelperMenu;

        private readonly Hero LocalHero = EntityManager.LocalHero;

        public TowerHelper(Common common)
        {
            TowerHelperMenu = common.MenuConfig.ShowMeMoreMenu.TowerHelperMenu;

            TowerHelperMenu.AutoAttackRangeItem.ValueChanged += OnAutoAttackRangeValueChanged;
        }

        public void Dispose()
        {
            TowerHelperMenu.AutoAttackRangeItem.ValueChanged -= OnAutoAttackRangeValueChanged;

            if (TowerHelperMenu.AutoAttackRangeItem)
            {
                UpdateManager.DestroyIngameUpdate(OnUpdate);
            }
        }

        private void OnAutoAttackRangeValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                UpdateManager.CreateIngameUpdate(400, OnUpdate);
            }
            else
            {
                UpdateManager.DestroyIngameUpdate(OnUpdate);

                foreach (var tower in EntityManager.GetEntities<Tower>())
                {
                    if (!tower.IsEnemy(LocalHero))
                    {
                        continue;
                    }

                    var id = $"TowerHelper_{tower.Handle}";
                    ParticleManager.RemoveParticle(id);
                }
            }
        }

        private void OnUpdate()
        {
            foreach (var tower in EntityManager.GetEntities<Tower>())
            {
                if (!tower.IsEnemy(LocalHero))
                {
                    continue;
                }

                var id = $"TowerHelper_{tower.Handle}";
                var position = tower.Position;
                if (LocalHero.Distance2D(position) < 1100)
                {
                    ParticleManager.CreateOrUpdateParticle(
                        id,
                        @"particles\ui_mouseactions\range_finder_tower_aoe_ring.vpcf",
                        tower,
                        ParticleAttachment.AbsOrigin,
                        new ControlPoint(2, position),
                        new ControlPoint(3, tower.AttackRange(LocalHero), 0, 0),
                        new ControlPoint(4, 255, 0, 0));
                }
                else
                {
                    ParticleManager.RemoveParticle(id);
                }
            }
        }
    }
}