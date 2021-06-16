using BeAware.MenuManager.ShowMeMore;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Update;

namespace BeAware.ShowMeMore
{
    internal sealed class LinkenShow
    {
        private readonly LinkenShowMenu LinkenShowMenu;

        private readonly Hero LocalHero = EntityManager.LocalHero;

        public LinkenShow(Common common)
        {
            LinkenShowMenu = common.MenuConfig.ShowMeMoreMenu.LinkenShowMenu;

            LinkenShowMenu.EnableItem.ValueChanged += OnEnableValueChanged;
        }

        public void Dispose()
        {
            if (LinkenShowMenu.EnableItem)
            {
                UpdateManager.DestroyIngameUpdate(OnUpdate);

                foreach (var hero in EntityManager.GetEntities<Hero>())
                {
                    if (hero.IsAlly(LocalHero))
                    {
                        continue;
                    }

                    RemoveLinkenShow(hero);
                }
            }
        }

        private void OnEnableValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                UpdateManager.CreateIngameUpdate(300, OnUpdate);
            }
            else
            {
                UpdateManager.DestroyIngameUpdate(OnUpdate);

                foreach (var hero in EntityManager.GetEntities<Hero>())
                {
                    if (hero.IsAlly(LocalHero))
                    {
                        continue;
                    }

                    RemoveLinkenShow(hero);
                }
            }
        }

        private void OnUpdate()
        {
            foreach (var hero in EntityManager.GetEntities<Hero>())
            {
                if (hero.IsAlly(LocalHero))
                {
                    continue;
                }

                if (hero.IsLinkensProtected() && hero.IsVisible && hero.IsAlive)
                {
                    AddLinkenShow(hero);
                }
                else
                {
                    RemoveLinkenShow(hero);
                }
            }
        }

        private void AddLinkenShow(Hero hero)
        {
            ParticleManager.CreateOrUpdateParticle($"LinkenShow_{hero.Handle}", "particles/items_fx/immunity_sphere_buff.vpcf", hero, ParticleAttachment.CenterFollow);
        }

        private void RemoveLinkenShow(Hero hero)
        {
            ParticleManager.RemoveParticle($"LinkenShow_{hero.Handle}");
        }
    }
}
