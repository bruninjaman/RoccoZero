using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.Unit;
using Divine.SDK.Extensions;
using Divine.SDK.Helpers;
using Divine.SkywrathMage.Menus;
using Divine.SkywrathMage.Menus.Combo;

using Ensage.SDK.Extensions;
using Ensage.SDK.Menu;

namespace Divine.SkywrathMage.Combos
{
    internal sealed class AutoArcaneBolt : BaseTaskHandler
    {
        private readonly ComboMenu ComboMenu;

        private readonly SmartArcaneBoltMenu SmartArcaneBoltMenu;

        private readonly Abilities Abilities;

        public AutoArcaneBolt(Common common)
        {
            ComboMenu = common.MenuConfig.ComboMenu;
            SmartArcaneBoltMenu = common.MenuConfig.MoreMenu.SmartArcaneBoltMenu;

            Abilities = (Abilities)common.Abilities;

            SmartArcaneBoltMenu.ToggleHotkey.Action += ToggleHotkeyAction;
        }

        public override void Dispose()
        {
            base.Dispose();

            SmartArcaneBoltMenu.ToggleHotkey.Action -= ToggleHotkeyAction;
        }

        private void ToggleHotkeyAction(MenuInputEventArgs agrs)
        {
            if (SmartArcaneBoltMenu.ToggleHotkeyItem)
            {
                RunAsync();
            }
            else
            {
                Cancel();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (ComboMenu.ComboHotkeyItem || SmartArcaneBoltMenu.SpamHotkeyItem)
                {
                    return;
                }

                if (IsStopped)
                {
                    return;
                }

                if (SmartArcaneBoltMenu.OwnerMinHealthItem.Value > ((float)Owner.Health / Owner.MaximumHealth) * 100)
                {
                    return;
                }

                var arcaneBolt = Abilities.ArcaneBolt;
                if (!arcaneBolt.CanBeCasted)
                {
                    return;
                }

                var target = UnitManager<CHero, Enemy, NoIllusion>.Units.Where(x => x.IsVisible && x.IsAlive && arcaneBolt.CanHit(x))
                                                                        .OrderBy(x => x.Health)
                                                                        .FirstOrDefault();

                if (IsNullTarget(target))
                {
                    return;
                }

                if (target.IsBlockMagicDamage(new TargetModifiers(target)) || Owner.IsInvisible())
                {
                    return;
                }

                arcaneBolt.Cast(target);
                var castDelay = arcaneBolt.GetCastDelay(target);
                var hitTime = arcaneBolt.GetHitTime(target) - (castDelay + 340);
                Helpers.MultiSleeper<string>.DelaySleep($"IsHitTime_{target.Name}_{arcaneBolt.Name}", castDelay + 40, hitTime);
                await Task.Delay(castDelay, token);
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
