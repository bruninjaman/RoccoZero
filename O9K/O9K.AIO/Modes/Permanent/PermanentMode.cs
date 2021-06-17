namespace O9K.AIO.Modes.Permanent
{
    using System;

    using Base;

    using Core.Logger;
    using Core.Managers.Menu.EventArgs;

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

    internal abstract class PermanentMode : BaseMode
    {
        protected readonly UpdateHandler Handler;

        private readonly PermanentModeMenu menu;

        protected PermanentMode(BaseHero baseHero, PermanentModeMenu menu)
            : base(baseHero)
        {
            this.UnitManager = baseHero.UnitManager;
            this.menu = menu;

            this.Handler = UpdateManager.CreateIngameUpdate(0, menu.Enabled, this.OnUpdate);
        }

        protected UnitManager UnitManager { get; }

        public virtual void Disable()
        {
            this.Handler.IsEnabled = false;
            this.menu.Enabled.ValueChange -= this.EnabledOnValueChanged;
        }

        public override void Dispose()
        {
            base.Dispose();
            UpdateManager.DestroyIngameUpdate(this.Handler);
            this.menu.Enabled.ValueChange -= this.EnabledOnValueChanged;
        }

        public virtual void Enable()
        {
            this.menu.Enabled.ValueChange += this.EnabledOnValueChanged;
        }

        protected abstract void Execute();

        private void EnabledOnValueChanged(object sender, SwitcherEventArgs e)
        {
            this.Handler.IsEnabled = e.NewValue;
        }

        private void OnUpdate()
        {
            if (GameManager.IsPaused)
            {
                return;
            }

            try
            {
                this.Execute();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}