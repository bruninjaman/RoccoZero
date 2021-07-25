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
    using Core.Managers.Menu.Items;
    using Divine.Game;
    using Divine.Order;
    using Divine.Update;
    using Divine.Order.EventArgs;
    using Divine.Order.Orders.Components;
    using Divine.Entity.Entities.Abilities.Components;

    using Heroes.Base;

    using UnitManager;

    using KeyEventArgs = Core.Managers.Menu.EventArgs.KeyEventArgs;


    internal class ComboMode : BaseMode, IComboMode
    {
        public Dictionary<MenuHoldKey, ComboModeMenu> ComboModeMenus { get; } =
            new Dictionary<MenuHoldKey, ComboModeMenu>();

        public List<uint> DisableToggleAbilities { get; } = new List<uint>();

        public HashSet<AbilityId> IgnoreToggleDisable { get; } = new HashSet<AbilityId>
        {
            AbilityId.troll_warlord_berserkers_rage
        };

        public UpdateHandler UpdateHandler { get; }

        public bool IgnoreComboEnd { get; set; }

        public ComboMode(BaseHero baseHero, IEnumerable<ComboModeMenu> comboMenus)
            : base(baseHero)
        {
            this.UnitManager = baseHero.UnitManager;
            this.UpdateHandler = UpdateManager.CreateIngameUpdate(0, false, this.OnUpdate);

            foreach (var comboMenu in comboMenus)
            {
                this.ComboModeMenus.Add(comboMenu.Key, comboMenu);
            }
        }

        public ComboModeMenu ComboModeMenu { get; set; }

        public IUnitManager UnitManager { get; }

        public virtual void Disable()
        {
            this.UpdateHandler.IsEnabled = false;
            OrderManager.OrderAdding -= this.OnOrderAdding;

            foreach (var comboMenu in this.ComboModeMenus)
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public override void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.UpdateHandler);
            OrderManager.OrderAdding -= this.OnOrderAdding;

            foreach (var comboMenu in this.ComboModeMenus)
            {
                comboMenu.Key.ValueChange -= this.KeyOnValueChanged;
            }
        }

        public virtual void Enable()
        {
            OrderManager.OrderAdding += this.OnOrderAdding;

            foreach (var comboMenu in this.ComboModeMenus)
            {
                comboMenu.Key.ValueChange += this.KeyOnValueChanged;
            }
        }

        public virtual void ComboEnd()
        {
            try
            {
                foreach (var abilityHandle in this.DisableToggleAbilities.Distinct().ToList())
                {
                    if (!(EntityManager9.GetAbility(abilityHandle) is IToggleable ability))
                    {
                        continue;
                    }

                    UpdateManager.BeginInvoke(() => this.ToggleAbility(ability));
                }

                this.UnitManager.EndCombo(this.ComboModeMenu);
                this.DisableToggleAbilities.Clear();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public virtual void OnUpdate()
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

        public virtual void KeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (e.NewValue)
            {
                if (this.UpdateHandler.IsEnabled)
                {
                    this.IgnoreComboEnd = true;
                }

                this.ComboModeMenu = this.ComboModeMenus[(MenuHoldKey)sender];
                this.TargetManager.TargetLocked = true;
                this.UpdateHandler.IsEnabled = true;
            }
            else
            {
                if (this.IgnoreComboEnd)
                {
                    this.IgnoreComboEnd = false;
                    return;
                }

                this.UpdateHandler.IsEnabled = false;
                this.TargetManager.TargetLocked = false;
                this.ComboEnd();
            }
        }

        public virtual void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (!this.UpdateHandler.IsEnabled || !e.Process || !e.IsCustom)
                {
                    return;
                }

                var order = e.Order;
                switch (order.Type)
                {
                    case OrderType.CastToggleAutocast:
                    case OrderType.CastToggle:
                        {
                            if (this.IgnoreToggleDisable.Contains(order.Ability.Id))
                            {
                                return;
                            }

                            this.DisableToggleAbilities.Add(order.Ability.Handle);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public virtual async void ToggleAbility(IToggleable toggle)
        {
            try
            {
                if (!toggle.Enabled)
                {
                    await Task.Delay(200);
                }

                while (toggle.IsValid && toggle.Enabled && !this.UpdateHandler.IsEnabled)
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