﻿namespace O9K.AIO.Heroes.EarthSpirit.Abilities
{
    using System.Linq;

    using AIO.Abilities;
    using AIO.Modes.Combo;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Extensions;
    using Core.Helpers;
    using Core.Managers.Entity;
    using Core.Prediction.Data;

    using O9K.Core.Geometry;

    using TargetManager;

    internal class BoulderSmash : NukeAbility
    {
        public BoulderSmash(ActiveAbility ability)
            : base(ability)
        {
        }

        public override bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
        {
            if (this.Owner.HasModifier("modifier_earth_spirit_rolling_boulder_caster"))
            {
                return false;
            }

            return true;
        }

        public override bool ForceUseAbility(TargetManager targetManager, Sleeper comboSleeper)
        {
            var target = targetManager.Target;

            var input = this.GetPredictionInput(target);
            var output = this.Ability.GetPredictionOutput(input);

            if (output.HitChance < HitChance.Low)
            {
                return false;
            }

            foreach (var unit in EntityManager9.Units.Where(
                x => x.IsUnit && (!x.IsHero || x.IsIllusion) && x.Distance(this.Owner) < this.Ability.CastRange && x.IsAlive))
            {
                var rec = new Polygon.Rectangle(
                    this.Owner.Position.Extend2D(unit.Position, -100),
                    this.Owner.Position.Extend2D(unit.Position, this.Ability.Range),
                    this.Ability.Radius);

                if (!rec.IsInside(output.TargetPosition))
                {
                    continue;
                }

                if (!this.Ability.UseAbility(unit))
                {
                    continue;
                }

                var delay = this.Ability.GetCastDelay(targetManager.Target);
                comboSleeper.Sleep(delay);
                this.Sleeper.Sleep(delay + 0.5f);
                this.OrbwalkSleeper.Sleep(delay);
                return true;
            }

            return false;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            var target = targetManager.Target;
            var input = this.GetPredictionInput(target);
            var output = this.Ability.GetPredictionOutput(input);

            if (output.HitChance < HitChance.Low)
            {
                return false;
            }

            foreach (var stone in EntityManager9.Units.Where(
                x => x.Name == "npc_dota_earth_spirit_stone" && x.Distance(this.Owner) < this.Ability.CastRange && x.IsAlive))
            {
                var rec = new Polygon.Rectangle(
                    this.Owner.Position.Extend2D(output.TargetPosition, -100),
                    this.Owner.Position.Extend2D(output.TargetPosition, this.Ability.Range),
                    this.Ability.Radius);

                if (!rec.IsInside(stone.Position))
                {
                    continue;
                }

                if (!this.Ability.UseAbility(output.CastPosition))
                {
                    continue;
                }

                var delay = this.Ability.GetCastDelay(targetManager.Target);
                comboSleeper.Sleep(delay);
                this.Sleeper.Sleep(delay + 0.5f);
                this.OrbwalkSleeper.Sleep(delay);
                return true;
            }

            return false;
        }

        private PredictionInput9 GetPredictionInput(Unit9 target)
        {
            var input = this.Ability.GetPredictionInput(target);
            input.SkillShotType = SkillShotType.Line;
            return input;
        }
    }
}