using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



using Divine.Core.ComboFactory.Menus;
using Divine.Core.Extensions;
using Divine.Core.Entities;
using Divine.Core.Entities.Abilities;

namespace Divine.Core.ComboFactory.Combos
{
    public abstract class BaseLinkenBreaker : BaseTaskHandler
    {
        private BaseLinkenBreakerMenu LinkenBreakerMenu { get; }

        public BaseLinkenBreaker(BaseMenuConfig menuConfig)
        {
            LinkenBreakerMenu = menuConfig.LinkenBreakerMenu;
        }

        public void RunAsync()
        {
            RunAsync(false);
        }

        protected abstract CAbility[] LinkenBreakerAbilities { get; }

        private string[] BreakerChanger(bool isSpellShieldProtected)
        {
            if (isSpellShieldProtected)
            {
                var prioritySpellShieldItem = LinkenBreakerMenu.PrioritySpellShieldItem;
                return prioritySpellShieldItem.Priority.Where(x => prioritySpellShieldItem.PictureStates[x]).ToArray();
            }

            var priorityLinkensItem = LinkenBreakerMenu.PriorityLinkensItem;
            return priorityLinkensItem.Priority.Where(x => priorityLinkensItem.PictureStates[x]).ToArray();
        }

        protected static HashSet<AbilityId> HitTimeAbilities { get; } = new HashSet<AbilityId>
        {
            AbilityId.item_nullifier,
            AbilityId.item_rod_of_atos
        };

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                var target = CurrentTarget;
                if (IsNullTarget(target))
                {
                    return;
                }

                foreach (var order in BreakerChanger(target.IsAntimageSpellShield()))
                {
                    foreach (var ability in LinkenBreakerAbilities)
                    {
                        if (ability == null || ability.Name != order || !ability.CanBeCasted)
                        {
                            continue;
                        }

                        if (ability is IActiveAbility activeAbility && activeAbility.CanHit(target))
                        {
                            ability.Cast(target);
                            await Task.Delay(HitTimeAbilities.Contains(ability.Id) ? activeAbility.GetHitTime(target) + 100 : activeAbility.GetCastDelay(target), token);
                            break;
                        }
                        else if (LinkenBreakerMenu.UseOnlyFromRangeItem)
                        {
                            break;
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
