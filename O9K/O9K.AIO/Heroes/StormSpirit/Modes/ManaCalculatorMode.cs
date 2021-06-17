namespace O9K.AIO.Heroes.StormSpirit.Modes
{
    using System;

    using AIO.Modes.Base;

    using Base;

    using Core.Entities.Abilities.Heroes.StormSpirit;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
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

    using Divine.Numerics;

    internal class ManaCalculatorMode : BaseMode
    {
        private readonly ManaCalculatorModeMenu menu;

        private BallLightning ballLightning;

        public ManaCalculatorMode(BaseHero baseHero, ManaCalculatorModeMenu menu)
            : base(baseHero)
        {
            this.menu = menu;
        }

        private BallLightning BallLightning
        {
            get
            {
                if (this.ballLightning?.IsValid != true)
                {
                    this.ballLightning = EntityManager9.GetAbility<BallLightning>(this.Owner.Hero);
                }

                return this.ballLightning;
            }
        }

        public void Disable()
        {
            this.menu.Enabled.ValueChange -= this.EnabledOnValueChanged;
        }

        public override void Dispose()
        {
            this.menu.Enabled.ValueChange -= this.EnabledOnValueChanged;
        }

        public void Enable()
        {
            this.menu.Enabled.ValueChange += this.EnabledOnValueChanged;
        }

        private void EnabledOnValueChanged(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                RendererManager.Draw += this.OnDraw;
            }
            else
            {
                RendererManager.Draw -= this.OnDraw;
            }
        }

        private void OnDraw()
        {
            try
            {
                if (this.BallLightning == null || this.ballLightning.Level <= 0 || !this.ballLightning.Owner.IsAlive)
                {
                    return;
                }

                var mousePosition = GameManager.MousePosition;
                var mp = this.menu.ShowRemainingMp
                             ? this.BallLightning.GetRemainingMana(mousePosition).ToString()
                             : this.BallLightning.GetRequiredMana(mousePosition).ToString();

                RendererManager.DrawText(
                    mp,
                    GameManager.MouseScreenPosition + (new Vector2(30, 30) * Hud.Info.ScreenRatio),
                    Color.White,
                    16 * Hud.Info.ScreenRatio);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}