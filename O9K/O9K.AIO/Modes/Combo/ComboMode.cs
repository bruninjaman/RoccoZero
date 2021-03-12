namespace O9K.AIO.Modes.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Base;

    using Core.Entities.Abilities.Base.Components;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine;

    using Heroes.Base;

    using UnitManager;

    using KeyEventArgs = Core.Managers.Menu.EventArgs.KeyEventArgs;

    internal class ComboMode : BaseMode
    {
        private readonly Dictionary<MenuHoldKey, ComboModeMenu> comboModeMenus = new Dictionary<MenuHoldKey, ComboModeMenu>();

        private readonly List<uint> disableToggleAbilities = new List<uint>();

        private readonly HashSet<AbilityId> ignoreToggleDisable = new HashSet<AbilityId>
        {
            AbilityId.troll_warlord_berserkers_rage
        };

        private readonly UpdateHandler updateHandler;

        private bool ignoreComboEnd;

        public ComboMode(BaseHero baseHero, IEnumerable<ComboModeMenu> comboMenus)
            : base(baseHero)
        {
            this.UnitManager = baseHero.UnitManager;
            this.updateHandler = UpdateManager.CreateIngameUpdate(0, false, this.OnUpdate);

            foreach (var comboMenu in comboMenus)
            {
                this.comboModeMenus.Add(comboMenu.Key, comboMenu);
            }
        }

        protected ComboModeMenu ComboModeMenu { get; private set; }

        protected UnitManager UnitManager { get; }

        public void Disable()
        {
            this.updateHandler.IsEnabled = false;
            OrderManager.OrderAdding -= this.OnOrderAdding;

            foreach (var comboMenu in this.comboModeMenus)
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public override void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.updateHandler);
            OrderManager.OrderAdding -= this.OnOrderAdding;

            foreach (var comboMenu in this.comboModeMenus)
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public void Enable()
        {
            OrderManager.OrderAdding += this.OnOrderAdding;

            foreach (var comboMenu in this.comboModeMenus)
            {
                comboMenu.Key.ValueChange += this.KeyOnValueChanged;
            }
        }

        protected void ComboEnd()
        {
            try
            {
                foreach (var abilityHandle in this.disableToggleAbilities.Distinct().ToList())
                {
                    if (!(EntityManager9.GetAbility(abilityHandle) is IToggleable ability))
                    {
                        continue;
                    }

                    UpdateManager.BeginInvoke(() => this.ToggleAbility(ability));
                }

                this.UnitManager.EndCombo(this.ComboModeMenu);
                this.disableToggleAbilities.Clear();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        protected void OnUpdate()
        {
            if (GameManager.IsPaused)
            {
                return;
            }

            try
            {
                if (this.TargetManager.HasValidTarget)
                {
                    this.UnitManager.ExecuteCombo(this.ComboModeMenu);
                }

                this.UnitManager.Orbwalk(this.ComboModeMenu);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void KeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (e.NewValue)
            {
                if (this.updateHandler.IsEnabled)
                {
                    this.ignoreComboEnd = true;
                }

                this.ComboModeMenu = this.comboModeMenus[(MenuHoldKey)sender];
                this.TargetManager.TargetLocked = true;
                this.updateHandler.IsEnabled = true;
            }
            else
            {
                if (this.ignoreComboEnd)
                {
                    this.ignoreComboEnd = false;
                    return;
                }

                this.updateHandler.IsEnabled = false;
                this.TargetManager.TargetLocked = false;
                this.ComboEnd();
            }
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (!this.updateHandler.IsEnabled || !e.Process || !e.IsCustom)
                {
                    return;
                }

                var order = e.Order;
                switch (order.Type)
                {
                    case OrderType.CastToggleAutocast:
                    case OrderType.CastToggle:
                        {
                            if (this.ignoreToggleDisable.Contains(order.Ability.Id))
                            {
                                return;
                            }

                            this.disableToggleAbilities.Add(order.Ability.Handle);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private async void ToggleAbility(IToggleable toggle)
        {
            try
            {
                if (!toggle.Enabled)
                {
                    await Task.Delay(200);
                }

                while (toggle.IsValid && toggle.Enabled && !this.updateHandler.IsEnabled)
                {
                    if (toggle.CanBeCasted() && !toggle.Owner.IsCasting)
                    {
                        toggle.Enabled = false;
                        break;
                    }

                    await Task.Delay(200);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}