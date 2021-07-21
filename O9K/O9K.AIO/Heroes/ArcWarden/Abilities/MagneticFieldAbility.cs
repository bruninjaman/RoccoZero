using System;
using O9K.AIO.Abilities;
using O9K.Core.Entities.Abilities.Base;

namespace O9K.AIO.Heroes.ArcWarden.Abilities
{
    using System.Linq;
    using Core.Extensions;
    using Core.Helpers;
    using Core.Managers.Entity;
    using Core.Prediction.Data;

    internal class MagneticFieldAbility : ShieldAbility
    {
        public MagneticFieldAbility(ActiveAbility ability)
            : base(ability)
        {
            this.field = (O9K.Core.Entities.Abilities.Heroes.ArcWarden.MagneticField) ability;
        }

        private readonly O9K.Core.Entities.Abilities.Heroes.ArcWarden.MagneticField field;

        public override bool ShouldCast(TargetManager.TargetManager targetManager)
        {
            var target = targetManager.Owner.Hero;

            if (target.IsInvulnerable)
            {
                return false;
            }

            if (target.Equals(this.Shield.Owner))
            {
                if (!this.Shield.ShieldsOwner)
                {
                    return false;
                }
            }
            else
            {
                if (!this.Shield.ShieldsAlly)
                {
                    return false;
                }
            }

            if (target.IsMagicImmune && !this.Shield.PiercesMagicImmunity(target))
            {
                return false;
            }

            if (target.HasModifier(this.Shield.ShieldModifierName))
            {
                return false;
            }

            if (this.Shield is ToggleAbility toggle && toggle.Enabled)
            {
                return false;
            }

            if (!base.ShouldCast(targetManager))
            {
                return false;
            }

            return true;
        }

        public override bool UseAbility(TargetManager.TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {

            var heroes = EntityManager9.Units.Where(x => x.IsHero && x.IsAlive && !x.IsIllusion && x.IsVisible)
                .ToList();

            var allies = heroes.Where(x => !x.IsInvulnerable && x.IsAlly(this.Owner))
                .ToList();
            var enemies = heroes.Where(x => !x.IsInvulnerable && x.IsEnemy(this.Owner))
                .ToList();


            var abilityOwner = this.Owner;
            var mainHero = EntityManager9.Owner;


            var isMainHero = abilityOwner.Equals(mainHero);

            if (abilityOwner.HasModifier(this.Shield.ShieldModifierName))
            {
                return false;
            }

            if (enemies.Count(x => x.Distance(abilityOwner) < abilityOwner.GetAttackRange()) < 1)
            {
                return false;
            }

            var closestEnemy = enemies.OrderBy(x => x.Distance(abilityOwner)).FirstOrDefault();
            
            if (closestEnemy != null && closestEnemy.Distance(mainHero) < 300 && abilityOwner.Distance(mainHero) < 800)
            {
                var position = mainHero.Hero.Position.Extend2D(closestEnemy.Position, -this.field.Radius);

                if (this.Owner.Distance(position) < this.Ability.CastRange && this.Ability.UseAbility(position))
                {
                    Sleeper.Sleep(1);
                    return true;
                }
            }
            
            
            if (closestEnemy != null && closestEnemy.Distance(abilityOwner) < 300)
            {
                var position = abilityOwner.Position.Extend2D(closestEnemy.Position, -this.field.Radius);

                if (this.Owner.Distance(position) < this.Ability.CastRange && this.Ability.UseAbility(position))
                {
                    Sleeper.Sleep(1);
                    return true;
                }

                return false;
            }

            return this.Ability.UseAbility(abilityOwner, allies, HitChance.Medium);
        }
    }
}