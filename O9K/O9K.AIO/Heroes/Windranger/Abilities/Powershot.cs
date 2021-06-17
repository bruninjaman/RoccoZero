namespace O9K.AIO.Heroes.Windranger.Abilities
{
    using System;
    using System.Collections.Generic;

    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Extensions;
    using Core.Helpers;

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

    using Modes.Combo;

    using O9K.Core.Geometry;

    using Divine.Numerics;

    using TargetManager;

    using BasePowershot = Core.Entities.Abilities.Heroes.Windranger.Powershot;

    internal class Powershot : NukeAbility, IDisposable
    {
        private readonly BasePowershot powershot;

        private Vector3 castPosition;

        public Powershot(ActiveAbility ability)
            : base(ability)
        {
            this.powershot = (BasePowershot)ability;
            OrderManager.OrderAdding += this.OnOrderAdding;
        }

        public Shackleshot Shackleshot { get; set; }

        public bool CancelChanneling(TargetManager targetManager)
        {
            if (!this.Ability.IsChanneling || !this.Ability.BaseAbility.IsChanneling)
            {
                return false;
            }

            var target = targetManager.Target;
            if (target.IsStunned || target.IsRooted)
            {
                return false;
            }

            var polygon = new Polygon.Rectangle(
                this.Owner.Position,
                this.Owner.Position.Extend2D(this.castPosition, this.Ability.Range),
                this.Ability.Radius - 75);

            var input = this.Ability.GetPredictionInput(target);
            input.Delay = this.powershot.ChannelTime - this.Ability.BaseAbility.ChannelTime;
            var output = this.Ability.GetPredictionOutput(input);

            if (!polygon.IsInside(output.TargetPosition) || this.powershot.GetCurrentDamage(target) > target.Health)
            {
                return this.Owner.BaseUnit.Stop();
            }

            return false;
        }

        public void Dispose()
        {
            OrderManager.OrderAdding -= this.OnOrderAdding;
        }

        public override bool ShouldConditionCast(TargetManager targetManager, IComboModeMenu menu, List<UsableAbility> usableAbilities)
        {
            var target = targetManager.Target;

            if (this.Ability.GetDamage(target) > target.Health)
            {
                return true;
            }

            if (usableAbilities.Count > 0)
            {
                return target.GetImmobilityDuration() > 0.4f;
            }

            return true;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            var target = targetManager.Target;
            var input = this.Ability.GetPredictionInput(target);

            if (this.Shackleshot?.Ability.TimeSinceCasted < 0.5f)
            {
                input.Delay -= this.Ability.ActivationDelay;
            }

            var output = this.Ability.GetPredictionOutput(input);

            if (!this.Ability.UseAbility(output.CastPosition))
            {
                return false;
            }

            var delay = this.Ability.GetCastDelay(targetManager.Target);
            comboSleeper.Sleep(delay);
            this.Sleeper.Sleep(delay + 0.5f);
            this.OrbwalkSleeper.Sleep(delay);
            return true;
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            var order = e.Order;
            if (order.IsQueued || order.Type != OrderType.CastPosition)
            {
                return;
            }

            if (order.Ability.Handle == this.Ability.Handle)
            {
                this.castPosition = order.Position;
            }
        }
    }
}