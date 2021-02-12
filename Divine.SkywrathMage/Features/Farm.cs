using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers;
using Divine.Core.Managers.Unit;
using Divine.Core.Services;

using Ensage.SDK.Extensions;
using Ensage.SDK.Menu;
using Ensage.SDK.Menu.ValueBinding;
using Ensage.SDK.Orbwalker.Config;
using Ensage.SDK.TargetSelector.Modes;

namespace Divine.SkywrathMage.Features
{
    internal sealed class Farm : BaseTaskHandler
    {
        private readonly BaseFarmMenu FarmMenu;

        private readonly AutoAttackModeConfig AutoAttackModeConfig;

        private readonly AutoAttackModeSelector Selector;

        private readonly Abilities Abilities;

        public Farm(Common common)
        {
            FarmMenu = common.MenuConfig.FarmMenu;

            Abilities = (Abilities)common.Abilities;

            var context = DivineService.Context;
            var parent = context.Orbwalker.Settings.Factory.Parent;
            AutoAttackModeConfig = new AutoAttackModeConfig(parent, "SkywrathMage Farm Mode", 0, true, false, false, false, true, true);
            Selector = new AutoAttackModeSelector(Owner.Base, context.TargetSelector, AutoAttackModeConfig);

            AutoAttackModeConfig.Active.Item.ShowItem = false;
            AutoAttackModeConfig.Key.Item.ShowItem = false;
            AutoAttackModeConfig.Deny.Item.ShowItem = false;
            AutoAttackModeConfig.Farm.Item.ShowItem = false;
            AutoAttackModeConfig.Hero.Item.ShowItem = false;
            AutoAttackModeConfig.Building.Item.ShowItem = false;
            AutoAttackModeConfig.Neutral.Item.ShowItem = false;
            AutoAttackModeConfig.Creep.Item.ShowItem = false;
            AutoAttackModeConfig.BonusMeleeRange.Item.ShowItem = false;
            AutoAttackModeConfig.BonusRangedRange.Item.ShowItem = false;

            FarmMenu.FarmHotkey.Action += FarmHotkeyAction;
            FarmMenu.HeroHarrasItem.ValueChanging += HeroHarrasChanging;
        }

        public override void Dispose()
        {
            base.Dispose();

            FarmMenu.HeroHarrasItem.ValueChanging -= HeroHarrasChanging;
            FarmMenu.FarmHotkey.Action -= FarmHotkeyAction;

            AutoAttackModeConfig?.Dispose();
        }

        private void FarmHotkeyAction(MenuInputEventArgs agrs)
        {
            if (agrs.Flag == HotkeyFlags.Down)
            {
                RunAsync();
            }
            else
            {
                Cancel();
            }
        }

        private void HeroHarrasChanging(object sender, ValueChangingEventArgs<string> e)
        {
            if (e.Value != "Disable")
            {
                AutoAttackModeConfig.Hero.Value = true;
            }
            else
            {
                AutoAttackModeConfig.Hero.Value = false;
            }
        }

        protected async override Task ExecuteAsync(CancellationToken token)
        {
            if (IsStopped)
            {
                return;
            }

            var arcaneBolt = Abilities.ArcaneBolt;
            if (FarmMenu.FarmItem.Value == "Arcane Bolt & Attack")
            {
                var unit = UnitManager<CUnit, Enemy, NoIllusion>.Units.FirstOrDefault(x =>
                                                                       (x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Creep_Neutral ||
                                                                       x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit ||
                                                                       x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Warlock_Golem ||
                                                                       x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Creep ||
                                                                       x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Creep_Lane ||
                                                                       x.NetworkClassId == NetworkClassId.CDOTA_BaseNPC_Creep_Siege ||
                                                                       x.NetworkClassId == NetworkClassId.CDOTA_Unit_Hero_Beastmaster_Boar ||
                                                                       x.NetworkClassId == NetworkClassId.CDOTA_Unit_SpiritBear ||
                                                                       x.NetworkClassId == NetworkClassId.CDOTA_Unit_Broodmother_Spiderling) &&
                                                                       x.IsVisible &&
                                                                       x.IsAlive &&
                                                                       x.IsSpawned &&
                                                                       x.Distance2D(Owner) < arcaneBolt.CastRange &&
                                                                       x.Health < arcaneBolt.GetDamage(x));

                // ArcaneBolt
                if (unit != null && arcaneBolt.CanBeCasted && arcaneBolt.CanHit(unit))
                {
                    arcaneBolt.UseAbility(unit);
                    await Task.Delay(arcaneBolt.GetCastDelay(unit), token);
                    return;
                }
            }

            var selectorTarget = Selector.GetTarget();
            if (selectorTarget != null)
            {
                var target = UnitManager.GetUnitByHandle(selectorTarget.Handle);
                if (target != null && FarmMenu.HeroHarrasItem.Value == "Arcane Bolt & Attack" && target is CHero)
                {
                    if (arcaneBolt.CanBeCasted && arcaneBolt.CanHit(target))
                    {
                        arcaneBolt.UseAbility(target);
                        await Task.Delay(arcaneBolt.GetCastDelay(target), token);
                        return;
                    }
                }

                Orbwalker.OrbwalkTo(target);
                return;
            }

            Orbwalker.MoveToMousePosition();
        }
    }
}
