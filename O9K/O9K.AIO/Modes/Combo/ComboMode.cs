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
    using Divine.Camera;
    using Divine.Entity;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.GameConsole;

    using Divine.Input;
    using Divine.Log;
    using Divine.Map;

    using Divine.Modifier;
    using Divine.Numerics;
    using Divine.Orbwalker;
    using Divine.Order;
    using Divine.Particle;
    using Divine.Projectile;
    using Divine.Renderer;
    using Divine.Service;
    using Divine.Update;
    using Divine.Entity.Entities;
    using Divine.Entity.EventArgs;
    using Divine.Game.EventArgs;
    using Divine.GameConsole.Exceptions;
    using Divine.Input.EventArgs;
    using Divine.Map.Components;
    using Divine.Menu.Animations;
    using Divine.Menu.Components;

    using Divine.Menu.Helpers;

    using Divine.Menu.Styles;
    using Divine.Modifier.EventArgs;
    using Divine.Modifier.Modifiers;
    using Divine.Order.EventArgs;
    using Divine.Order.Orders;
    using Divine.Particle.Components;
    using Divine.Particle.EventArgs;
    using Divine.Particle.Particles;
    using Divine.Plugins.Humanizer;
    using Divine.Projectile.EventArgs;
    using Divine.Projectile.Projectiles;
    using Divine.Renderer.ValveTexture;
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.EventArgs;
    using Divine.Entity.Entities.Exceptions;
    using Divine.Entity.Entities.PhysicalItems;
    using Divine.Entity.Entities.Players;
    using Divine.Entity.Entities.Runes;
    using Divine.Entity.Entities.Trees;
    using Divine.Entity.Entities.Units;
    using Divine.Modifier.Modifiers.Components;
    using Divine.Modifier.Modifiers.Exceptions;
    using Divine.Order.Orders.Components;
    using Divine.Particle.Particles.Exceptions;
    using Divine.Projectile.Projectiles.Components;
    using Divine.Projectile.Projectiles.Exceptions;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Abilities.Spells;
    using Divine.Entity.Entities.Players.Components;
    using Divine.Entity.Entities.Runes.Components;
    using Divine.Entity.Entities.Units.Buildings;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Entity.Entities.Units.Creeps;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Wards;
    using Divine.Entity.Entities.Abilities.Items.Components;
    using Divine.Entity.Entities.Abilities.Items.Neutrals;
    using Divine.Entity.Entities.Abilities.Spells.Abaddon;
    using Divine.Entity.Entities.Abilities.Spells.Components;
    using Divine.Entity.Entities.Units.Creeps.Neutrals;
    using Divine.Entity.Entities.Units.Heroes.Components;

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