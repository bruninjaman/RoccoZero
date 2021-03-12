namespace O9K.AIO.Heroes.Windranger.Abilities
{
    using System;
    using System.Collections.Generic;

    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Extensions;
    using Core.Helpers;

    using Divine;

    using Modes.Combo;

    using O9K.Core.Geometry;

    using SharpDX;

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