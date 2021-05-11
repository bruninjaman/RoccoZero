namespace O9K.ItemManager.Modules.AutoActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Heroes;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Assembly;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine;
    using Divine.SDK.Localization;

    using Metadata;

    using O9K.Core.Managers.Context;

    using OrderHelper;

    using SharpDX;

    using Attribute = Divine.Attribute;

    internal class PowerTreads : IModule
    {
        private const int Delay = 50;

        private readonly MenuAbilityToggler agiToggler;

        private readonly Sleeper changeBackSleeper = new Sleeper();

        private readonly MenuSwitcher enabled;

        private readonly IAssemblyEventManager9 eventManager;

        private readonly HashSet<AbilityId> forceChangeBackAbilities = new HashSet<AbilityId>
        {
            AbilityId.naga_siren_mirror_image,
            AbilityId.chaos_knight_phantasm,
            AbilityId.item_manta,
        };

        private readonly Sleeper forceChangeBackSleeper = new Sleeper();

        private readonly HashSet<AbilityId> ignoredAbilities = new HashSet<AbilityId>
        {
            AbilityId.item_tpscroll,
            AbilityId.item_travel_boots,
            AbilityId.item_travel_boots_2
        };

        private readonly MenuAbilityToggler intToggler;

        private readonly MenuSwitcher manualOnly;

        private readonly IOrderSync orderSync;

        private readonly MenuHoldKey recoveryKey;

        private Attribute defaultAttribute;

        private bool ignoreNextOrder;

        private Owner owner;

        private Core.Entities.Abilities.Items.PowerTreads powerTreads;

        private bool subscribed;

        private bool switchingThreads;

        public PowerTreads(IMainMenu mainMenu, IOrderSync orderSync)
        {
            this.orderSync = orderSync;
            this.eventManager = Context9.AssemblyEventManager;

            var menu = mainMenu.AutoActionsMenu.Add(new Menu(LocalizationHelper.LocalizeName(AbilityId.item_power_treads), "PowerTreads"));

            this.enabled = menu.Add(new MenuSwitcher("Enabled"));
            this.enabled.AddTranslation(Lang.Ru, "Включено");
            this.enabled.AddTranslation(Lang.Cn, "启用");

            this.manualOnly = menu.Add(new MenuSwitcher("Manual only", false).SetTooltip("Use only when abilities are casted manually"));
            this.manualOnly.AddTranslation(Lang.Ru, "Только вручную");
            this.manualOnly.AddTranslation(Lang.Cn, "仅手册");
            this.manualOnly.AddTooltipTranslation(Lang.Ru, "Использовать только когда способности кастуются вручную");
            this.manualOnly.AddTooltipTranslation(Lang.Cn, "仅在手动使用技能时使用");

            this.intToggler = menu.Add(new MenuAbilityToggler("Intelligence"));
            this.intToggler.AddTranslation(Lang.Ru, "Инт");
            this.intToggler.AddTranslation(Lang.Cn, "智力");

            this.agiToggler = menu.Add(new MenuAbilityToggler("Agility"));
            this.agiToggler.AddTranslation(Lang.Ru, "Агила");
            this.agiToggler.AddTranslation(Lang.Cn, "敏捷");

            // get recovery key
            this.recoveryKey = mainMenu.RecoveryAbuseMenu.GetOrAdd(new MenuHoldKey("Key"));
        }

        public void Activate()
        {
            this.owner = EntityManager9.Owner;

            this.enabled.ValueChange += this.EnabledOnValueChange;
        }

        public void Dispose()
        {
            this.enabled.ValueChange -= this.EnabledOnValueChange;
            this.eventManager.AutoSoulRingEnabled -= this.OnAutoSoulRingEnabled;
            EntityManager9.AbilityAdded -= this.OnAbilityAdded;
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
            OrderManager.OrderAdding -= this.OnOrderAdding;
            OrderManager.OrderAdding -= this.OnPowerTreadsOrderAdding;
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
            this.subscribed = false;
        }

        private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                EntityManager9.AbilityAdded += this.OnAbilityAdded;
                EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
                this.eventManager.AutoSoulRingEnabled += this.OnAutoSoulRingEnabled;
            }
            else
            {
                EntityManager9.AbilityAdded -= this.OnAbilityAdded;
                EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
                OrderManager.OrderAdding -= this.OnOrderAdding;
                OrderManager.OrderAdding -= this.OnPowerTreadsOrderAdding;
                this.eventManager.AutoSoulRingEnabled -= this.OnAutoSoulRingEnabled;
                UpdateManager.DestroyIngameUpdate(this.OnUpdate);
                this.subscribed = false;
            }
        }

        private int GetPowerTreadsSwitchCount(Ability9 ability, Vector3 position)
        {
            var currentAttribute = this.powerTreads.ActiveAttribute;

            if (this.intToggler.IsEnabled(ability.Name))
            {
                if (currentAttribute != Attribute.Intelligence)
                {
                    return currentAttribute == Attribute.Agility ? 2 : 1;
                }
            }
            else if (this.agiToggler.IsEnabled(ability.Name))
            {
                if (currentAttribute != Attribute.Agility)
                {
                    return currentAttribute == Attribute.Strength ? 2 : 1;
                }
            }

            if (currentAttribute != this.defaultAttribute)
            {
                this.SetSleep(ability, position);
            }

            return 0;
        }

        private void OnAbilityAdded(Ability9 ability)
        {
            try
            {
                if (!ability.Owner.IsMyHero || !(ability is ActiveAbility))
                {
                    return;
                }

                if (ability.Id == AbilityId.item_power_treads)
                {
                    if (this.subscribed)
                    {
                        return;
                    }

                    this.powerTreads = (Core.Entities.Abilities.Items.PowerTreads)ability;
                    this.defaultAttribute = this.powerTreads.ActiveAttribute;

                    OrderManager.OrderAdding += this.OnPowerTreadsOrderAdding;
                    OrderManager.OrderAdding += this.OnOrderAdding;
                    UpdateManager.CreateIngameUpdate(112, this.OnUpdate);
                    this.eventManager.InvokeForceBlockerResubscribe();
                    this.subscribed = true;
                }
                else if (ability.BaseAbility.AbilityData.GetManaCost(1) > 0 && !this.ignoredAbilities.Contains(ability.Id))
                {
                    this.intToggler.AddAbility(ability.Id);
                    this.agiToggler.AddAbility(ability.Id, false);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAbilityRemoved(Ability9 ability)
        {
            try
            {
                if (!ability.Owner.IsMyHero)
                {
                    return;
                }

                if (ability.Handle != this.powerTreads?.Handle)
                {
                    return;
                }

                OrderManager.OrderAdding -= this.OnOrderAdding;
                OrderManager.OrderAdding -= this.OnPowerTreadsOrderAdding;
                UpdateManager.DestroyIngameUpdate(this.OnUpdate);
                this.subscribed = false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAutoSoulRingEnabled(object sender, EventArgs e)
        {
            if (!this.subscribed)
            {
                return;
            }

            // change execute order check
            OrderManager.OrderAdding -= this.OnOrderAdding;
            OrderManager.OrderAdding += this.OnOrderAdding;
            this.eventManager.InvokeForceBlockerResubscribe();
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (this.ignoreNextOrder)
                {
                    this.ignoreNextOrder = false;
                    return;
                }

                if (!e.Process || this.recoveryKey)
                {
                    return;
                }

                var order = e.Order;
                if (order.IsQueued)
                {
                    return;
                }

                var isPlayerInput = !e.IsCustom;

                if (this.orderSync.ForceNextOrderManual)
                {
                    isPlayerInput = true;
                    this.orderSync.ForceNextOrderManual = false;
                }

                if (this.manualOnly && !isPlayerInput)
                {
                    return;
                }

                if (!order.Units.Contains(this.owner))
                {
                    return;
                }

                switch (order.Type)
                {
                    case OrderType.Cast:
                        {
                            if (order.Ability.Id == AbilityId.item_power_treads)
                            {
                                return;
                            }

                            if (this.switchingThreads)
                            {
                                e.Process = false;
                                return;
                            }

                            var ability = EntityManager9.GetAbility(order.Ability.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            if (this.PowerTreadsSwitched(ability, false, isPlayerInput))
                            {
                                e.Process = false;
                            }

                            break;
                        }
                    case OrderType.CastPosition:
                        {
                            if (this.switchingThreads)
                            {
                                e.Process = false;
                                return;
                            }

                            var ability = EntityManager9.GetAbility(order.Ability.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            if (this.PowerTreadsSwitched(ability, order.Position, isPlayerInput))
                            {
                                e.Process = false;
                            }

                            break;
                        }
                    case OrderType.CastTarget:
                        {
                            if (this.switchingThreads)
                            {
                                e.Process = false;
                                return;
                            }

                            var ability = EntityManager9.GetAbility(order.Ability.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            if (this.PowerTreadsSwitched(ability, (Unit)order.Target, isPlayerInput))
                            {
                                e.Process = false;
                            }

                            break;
                        }
                    case OrderType.CastToggle:
                        {
                            if (order.Ability.IsToggled)
                            {
                                return;
                            }

                            if (this.switchingThreads)
                            {
                                e.Process = false;
                                return;
                            }

                            var ability = EntityManager9.GetAbility(order.Ability.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            if (this.PowerTreadsSwitched(ability, true, isPlayerInput))
                            {
                                e.Process = false;
                            }

                            break;
                        }
                    case OrderType.CastRune:
                        {
                            if (this.switchingThreads)
                            {
                                e.Process = false;
                                return;
                            }

                            var ability = EntityManager9.GetAbility(order.Ability.Handle);
                            if (ability == null)
                            {
                                return;
                            }

                            if (this.PowerTreadsSwitched(ability, (Rune)order.Target, isPlayerInput))
                            {
                                e.Process = false;
                            }

                            break;
                        }
                        //case OrderId.AbilityTargetTree:
                        //{
                        //    break;
                        //}
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnPowerTreadsOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (!e.Process || e.IsCustom)
                {
                    return;
                }

                var order = e.Order;
                if (order.Type != OrderType.Cast || order.Ability.Handle != this.powerTreads.Handle)
                {
                    return;
                }

                if (!order.Units.Contains(this.owner))
                {
                    return;
                }

                if (!this.powerTreads.CanBeCasted())
                {
                    return;
                }

                this.powerTreads.UseAbility();
                this.defaultAttribute = this.powerTreads.ActiveAttribute;
                e.Process = false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnUpdate()
        {
            try
            {
                if (GameManager.IsPaused || this.changeBackSleeper || this.switchingThreads || this.recoveryKey)
                {
                    return;
                }

                if (!this.powerTreads.IsValid || this.powerTreads.ActiveAttribute == this.defaultAttribute
                                              || !this.powerTreads.CanBeCasted())
                {
                    return;
                }

                if ((this.powerTreads.Owner.IsCasting || this.powerTreads.Owner.IsInvulnerable || this.powerTreads.Owner.IsInvisible)
                    && !this.forceChangeBackSleeper)
                {
                    return;
                }

                this.powerTreads.UseAbility();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private bool PowerTreadsSwitched(Ability9 ability, Unit target, bool isPlayerInput)
        {
            if (!this.ShouldChangePowerTreads(ability))
            {
                return false;
            }

            var switchCount = this.GetPowerTreadsSwitchCount(ability, target.Position);
            if (switchCount == 0)
            {
                return false;
            }

            this.switchingThreads = true;

            UpdateManager.BeginInvoke(
                async () =>
                    {
                        try
                        {
                            await this.SwitchPowerTreads(switchCount);

                            //if (isPlayerInput)
                            {
                                this.ignoreNextOrder = true;
                                this.orderSync.IgnoreSoulRingOrder = true;
                                ability.BaseAbility.Cast(target);
                                this.SetSleep(ability, target.Position);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    });

            if (!this.changeBackSleeper)
            {
                this.changeBackSleeper.Sleep(0.5f);
            }

            return true;
        }

        private bool PowerTreadsSwitched(Ability9 ability, Vector3 position, bool isPlayerInput)
        {
            if (!this.ShouldChangePowerTreads(ability))
            {
                return false;
            }

            var switchCount = this.GetPowerTreadsSwitchCount(ability, position);
            if (switchCount == 0)
            {
                return false;
            }

            this.switchingThreads = true;

            UpdateManager.BeginInvoke(
                async () =>
                    {
                        try
                        {
                            await this.SwitchPowerTreads(switchCount);

                            //if (isPlayerInput)
                            {
                                this.ignoreNextOrder = true;
                                this.orderSync.IgnoreSoulRingOrder = true;
                                ability.BaseAbility.Cast(position);
                                this.SetSleep(ability, position);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    });

            if (!this.changeBackSleeper)
            {
                this.changeBackSleeper.Sleep(0.5f);
            }

            return true;
        }

        private bool PowerTreadsSwitched(Ability9 ability, Rune target, bool isPlayerInput)
        {
            if (!this.ShouldChangePowerTreads(ability))
            {
                return false;
            }

            var switchCount = this.GetPowerTreadsSwitchCount(ability, target.Position);
            if (switchCount == 0)
            {
                return false;
            }

            this.switchingThreads = true;

            UpdateManager.BeginInvoke(
                async () =>
                    {
                        try
                        {
                            await this.SwitchPowerTreads(switchCount);

                            //if (isPlayerInput)
                            {
                                this.ignoreNextOrder = true;
                                this.orderSync.IgnoreSoulRingOrder = true;
                                ability.BaseAbility.Cast(target);
                                this.SetSleep(ability, target.Position);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    });

            if (!this.changeBackSleeper)
            {
                this.changeBackSleeper.Sleep(0.5f);
            }

            return true;
        }

        private bool PowerTreadsSwitched(Ability9 ability, bool toggle, bool isPlayerInput)
        {
            if (!this.ShouldChangePowerTreads(ability))
            {
                return false;
            }

            var switchCount = this.GetPowerTreadsSwitchCount(ability, Vector3.Zero);
            if (switchCount == 0)
            {
                return false;
            }

            this.switchingThreads = true;

            UpdateManager.BeginInvoke(
                async () =>
                    {
                        try
                        {
                            await this.SwitchPowerTreads(switchCount);

                            //if (isPlayerInput)
                            {
                                this.ignoreNextOrder = true;
                                this.orderSync.IgnoreSoulRingOrder = true;

                                if (toggle)
                                {
                                    ability.BaseAbility.CastToggle();
                                }
                                else
                                {
                                    ability.BaseAbility.Cast();
                                }

                                this.SetSleep(ability, Vector3.Zero);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    });

            if (!this.changeBackSleeper)
            {
                this.changeBackSleeper.Sleep(0.5f);
            }

            return true;
        }

        private void SetSleep(Ability9 ability, Vector3 position)
        {
            if (this.forceChangeBackAbilities.Contains(ability.Id))
            {
                this.forceChangeBackSleeper.Sleep(0.3f);
                this.changeBackSleeper.Sleep(ability.IsItem ? 0.01f : 0.1f);
                return;
            }

            var delay = ability.CastPoint;

            if (!position.IsZero)
            {
                delay += ability.Owner.GetTurnTime(position);
            }

            this.changeBackSleeper.Sleep(delay + 0.5f);
        }

        private bool ShouldChangePowerTreads(Ability ability)
        {
            if (!this.powerTreads.IsValid)
            {
                return false;
            }

            if (ability.ManaCost <= 0)
            {
                return false;
            }

            if (!this.powerTreads.CanBeCasted(false) || this.powerTreads.Owner.IsInvulnerable || this.powerTreads.Owner.IsInvisible)
            {
                return false;
            }

            return true;
        }

        private async Task SwitchPowerTreads(int count)
        {
            this.switchingThreads = true;

            this.powerTreads.ChangeExpectedAttribute(count == 1);

            for (var i = 0; i < count; i++)
            {
                this.powerTreads.UseAbilitySimple();
                await Task.Delay(Delay);
            }

            this.switchingThreads = false;
        }
    }
}