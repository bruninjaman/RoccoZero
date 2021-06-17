namespace O9K.AIO.Heroes.EmberSpirit.Modes
{
    using System;
    using System.Linq;

    using AIO.Modes.Permanent;

    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Logger;

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

    internal class AutoChainsMode : PermanentMode
    {
        private readonly AutoChainsModeMenu menu;

        private bool useChains;

        public AutoChainsMode(BaseHero baseHero, AutoChainsModeMenu menu)
            : base(baseHero, menu)
        {
            this.menu = menu;
            OrderManager.OrderAdding += OnOrderAdding;
        }

        public override void Dispose()
        {
            base.Dispose();
            OrderManager.OrderAdding -= OnOrderAdding;
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        }

        protected override void Execute()
        {
            if (!this.useChains)
            {
                return;
            }

            var hero = this.Owner.Hero;

            if (!hero.IsValid || !hero.IsAlive)
            {
                return;
            }

            var chains = hero.Abilities.FirstOrDefault(x => x.Id == AbilityId.ember_spirit_searing_chains) as ActiveAbility;
            if (chains?.IsValid != true || !chains.CanBeCasted())
            {
                return;
            }

            var enemy = this.TargetManager.EnemyHeroes.Any(x => chains.CanHit(x));
            if (!enemy)
            {
                return;
            }

            if (this.menu.fistKey)
            {
                chains.UseAbility();
            }

            this.useChains = false;
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (e.IsCustom || !e.Process)
                {
                    return;
                }

                var order = e.Order;
                if (order.Type != OrderType.CastPosition)
                {
                    return;
                }

                if (order.Ability.Id == AbilityId.ember_spirit_sleight_of_fist)
                {
                    ModifierManager.ModifierAdded += this.OnModifierAdded;
                    ModifierManager.ModifierRemoved += this.OnModifierRemoved;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_ember_spirit_sleight_of_fist_caster")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    if (modifier.Owner.Handle != this.Owner.HeroHandle)
                    {
                        return;
                    }

                    this.useChains = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_ember_spirit_sleight_of_fist_caster")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    if (modifier.Owner.Handle != this.Owner.HeroHandle)
                    {
                        return;
                    }

                    ModifierManager.ModifierAdded -= this.OnModifierAdded;
                    ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
                    this.useChains = false;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }
    }
}