using System.Linq;

using Divine.Core.ComboFactory.Helpers;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Extensions;
using Divine.SDK.Extensions;
using Divine.SDK.Managers.Update;

namespace Divine.Core.ComboFactory.Combos
{
    public abstract class BaseKillSteal : BaseTaskHandler
    {
        protected readonly BaseKillStealMenu KillStealMenu;

        private UpdateHandler UpdateHandler;

        public BaseKillSteal(BaseMenuConfig menuConfig)
        {
            KillStealMenu = menuConfig.KillStealMenu;

            if (KillStealMenu.EnableItem)
            {
                UpdateHandler = UpdateManager.Subscribe(0, false, Stop);
                RunAsync();
            }

            KillStealMenu.EnableItem.Changed += EnableChanged;
        }

        public override void Dispose()
        {
            KillStealMenu.EnableItem.Changed -= EnableChanged;

            if (KillStealMenu.EnableItem)
            {
                base.Dispose();
                UpdateManager.Unsubscribe(Stop);
            }
        }

        private void EnableChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                UpdateHandler = UpdateManager.Subscribe(0, false, Stop);
                RunAsync();
            }
            else
            {
                Cancel();
                UpdateManager.Unsubscribe(Stop);
            }
        }

        public bool IsKillSteal { get; private set; }

        private Hero Target { get; set; }

        private float Damage { get; set; }

        protected override Unit CurrentTarget
        {
            get
            {
                IsKillSteal = false;
                var damageDate = BaseDamageCalculation.DamageDate.Where(x => x.IsKillSteal).OrderByDescending(x => x.GetHealth).FirstOrDefault();

                Target = null;
                if (damageDate != null)
                {
                    IsKillSteal = true;
                    Target = damageDate.GetHero;
                    Damage = damageDate.GetCanBeCastedDamage;
                }
                
                return Target;
            }
        }

        protected void StopCast()
        {
            if (!UpdateHandler.IsEnabled)
            {
                UpdateHandler.IsEnabled = true;
            }
        }

        private void Stop()
        {
            if (IsNullTarget(Target))
            {
                Target = null;
                UpdateHandler.IsEnabled = false;
                return;
            }

            var localHero = EntityManager.LocalHero;
            var stop = EntityManager.GetEntities<Hero>().Any(x => !x.IsAlly(localHero) && x == Target && (x.IsBlockMagicDamage() || !x.IsAlive || x.Health - Damage > 0));
            if (stop && Owner.AnimationName.Contains("cast"))
            {
                Target = null;
                Owner.Stop();
                UpdateHandler.IsEnabled = false;
            }
        }

        protected bool Reincarnation(Unit target)
        {
            var reincarnation = target.GetAbilityById(AbilityId.skeleton_king_reincarnation);
            return reincarnation != null && reincarnation.Cooldown == 0 && reincarnation.Level > 0;
        }
    }
}