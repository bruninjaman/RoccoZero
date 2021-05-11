namespace O9K.ItemManager.Modules.AutoActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Heroes;
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

    internal class SoulRing : IModule
    {
        private const int Delay = 50;

        private readonly MenuSwitcher enabled;

        private readonly IAssemblyEventManager9 eventManager;

        private readonly MenuSlider hpThreshold;

        private readonly HashSet<AbilityId> ignoredAbilities = new HashSet<AbilityId>
        {
            AbilityId.item_tpscroll,
            AbilityId.item_travel_boots,
            AbilityId.item_travel_boots_2
        };

        private readonly MenuSwitcher manualOnly;

        private readonly IOrderSync orderSync;

        private readonly MenuHoldKey recoveryKey;

        private readonly MenuAbilityToggler toggler;

        private bool ignoreNextOrder;

        private Owner owner;

        private ActiveAbility soulRing;

        private bool subscribed;

        public SoulRing(IMainMenu mainMenu, IOrderSync orderSync)
        {
            this.orderSync = orderSync;
            this.eventManager = Context9.AssemblyEventManager;

            var menu = mainMenu.AutoActionsMenu.Add(new Menu(LocalizationHelper.LocalizeName(AbilityId.item_soul_ring), "SoulRing"));

            this.enabled = menu.Add(new MenuSwitcher("Enabled"));
            this.enabled.AddTranslation(Lang.Ru, "Включено");
            this.enabled.AddTranslation(Lang.Cn, "启用");

            this.manualOnly = menu.Add(new MenuSwitcher("Manual only", false).SetTooltip("Use only when abilities are casted manually"));
            this.manualOnly.AddTranslation(Lang.Ru, "Только вручную");
            this.manualOnly.AddTranslation(Lang.Cn, "仅手册");
            this.manualOnly.AddTooltipTranslation(Lang.Ru, "Использовать только когда способности кастуются вручную");
            this.manualOnly.AddTooltipTranslation(Lang.Cn, "仅在手动使用技能时使用");

            this.toggler = menu.Add(new MenuAbilityToggler("Abilities"));
            this.toggler.AddTranslation(Lang.Ru, "Способности");
            this.toggler.AddTranslation(Lang.Cn, "播放声音");

            this.hpThreshold = menu.Add(new MenuSlider("Health%", 30, 0, 100));
            this.hpThreshold.AddTranslation(Lang.Ru, "Здоровье%");
            this.hpThreshold.AddTranslation(Lang.Cn, "生命值％");

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
            EntityManager9.AbilityAdded -= this.OnAbilityAdded;
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
            OrderManager.OrderAdding -= OnOrderAdding;
            this.subscribed = false;
        }

        private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                EntityManager9.AbilityAdded += this.OnAbilityAdded;
                EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
            }
            else
            {
                EntityManager9.AbilityAdded -= this.OnAbilityAdded;
                EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
                OrderManager.OrderAdding -= OnOrderAdding;
                this.subscribed = false;
            }
        }

        private void OnAbilityAdded(Ability9 ability)
        {
            try
            {
                if (!ability.Owner.IsMyHero || !(ability is ActiveAbility active))
                {
                    return;
                }

                if (ability.Id == AbilityId.item_soul_ring)
                {
                    if (this.subscribed)
                    {
                        return;
                    }

                    this.soulRing = active;
                    OrderManager.OrderAdding += OnOrderAdding;
                    this.eventManager.InvokeForceBlockerResubscribe();
                    this.eventManager.InvokeAutoSoulRingEnabled();
                    this.subscribed = true;
                }
                else if (ability.BaseAbility.AbilityData.GetManaCost(1) > 0 && !this.ignoredAbilities.Contains(ability.Id))
                {
                    this.toggler.AddAbility(ability.Id);
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

                if (ability.Handle != this.soulRing?.Handle)
                {
                    return;
                }

                OrderManager.OrderAdding -= OnOrderAdding;
                this.subscribed = false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
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

                if (this.orderSync.IgnoreSoulRingOrder)
                {
                    this.orderSync.IgnoreSoulRingOrder = false;
                    return;
                }

                var order = e.Order;
                if (!e.Process || order.IsQueued || this.recoveryKey)
                {
                    return;
                }

                if (this.manualOnly && e.IsCustom)
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
                            if (order.Ability.Id == AbilityId.item_soul_ring)
                            {
                                return;
                            }

                            if (this.SoulRingUsed(order.Ability, false, !e.IsCustom))
                            {
                                e.Process = false;
                            }

                            break;
                        }
                    case OrderType.CastPosition:
                        {
                            if (this.SoulRingUsed(order.Ability, order.Position, !e.IsCustom))
                            {
                                e.Process = false;
                            }

                            break;
                        }
                    case OrderType.CastTarget:
                        {
                            if (this.SoulRingUsed(order.Ability, (Unit)order.Target, !e.IsCustom))
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

                            if (this.SoulRingUsed(order.Ability, true, !e.IsCustom))
                            {
                                e.Process = false;
                            }

                            break;
                        }
                    case OrderType.CastRune:
                        {
                            if (this.SoulRingUsed(order.Ability, (Rune)order.Target, !e.IsCustom))
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

        private bool ShouldUseSoulRing(Ability ability)
        {
            if (!this.soulRing.CanBeCasted(false))
            {
                return false;
            }

            if (ability.ManaCost <= 0 || !this.toggler.IsEnabled(ability.Name))
            {
                return false;
            }

            if (!this.owner.Hero.CanBeHealed || this.owner.Hero.HealthPercentage < this.hpThreshold)
            {
                return false;
            }

            return true;
        }

        private bool SoulRingUsed(Ability ability, Vector3 position, bool isPlayerInput)
        {
            if (!this.ShouldUseSoulRing(ability))
            {
                return false;
            }

            this.soulRing.UseAbility();

            //if (isPlayerInput)
            {
                UpdateManager.BeginInvoke(Delay, () =>
                {
                    this.ignoreNextOrder = true;
                    this.orderSync.ForceNextOrderManual = true;
                    ability.Cast(position);
                });
            }

            return true;
        }

        private bool SoulRingUsed(Ability ability, Unit target, bool isPlayerInput)
        {
            if (!this.ShouldUseSoulRing(ability))
            {
                return false;
            }

            this.soulRing.UseAbility();

            //if (isPlayerInput)
            {
                UpdateManager.BeginInvoke(Delay, () =>
                {
                    this.ignoreNextOrder = true;
                    this.orderSync.ForceNextOrderManual = true;
                    ability.Cast(target);
                });
            }

            return true;
        }

        private bool SoulRingUsed(Ability ability, Rune rune, bool isPlayerInput)
        {
            if (!this.ShouldUseSoulRing(ability))
            {
                return false;
            }

            this.soulRing.UseAbility();

            //if (isPlayerInput)
            {
                UpdateManager.BeginInvoke(Delay, () =>
                {
                    this.ignoreNextOrder = true;
                    this.orderSync.ForceNextOrderManual = true;
                    ability.Cast(rune);
                });
            }

            return true;
        }

        private bool SoulRingUsed(Ability ability, bool toggle, bool isPlayerInput)
        {
            if (!this.ShouldUseSoulRing(ability))
            {
                return false;
            }

            this.soulRing.UseAbility();

            //if (isPlayerInput)
            {
                UpdateManager.BeginInvoke(Delay, () =>
                {
                    this.ignoreNextOrder = true;
                    this.orderSync.ForceNextOrderManual = true;

                    if (toggle)
                    {
                        ability.CastToggle();
                    }
                    else
                    {
                        ability.Cast();
                    }
                });
            }

            return true;
        }
    }
}