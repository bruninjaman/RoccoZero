namespace O9K.AIO.Heroes.ArcWarden.Abilities
{
    using System.Linq;
    using AIO.Abilities;
    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Heroes.ArcWarden;
    using Core.Extensions;
    using Core.Helpers;
    using Core.Managers.Entity;
    using Core.Prediction.Data;
    using Modes.Combo;
    using TargetManager;

    internal class MagneticFieldAbility : ShieldAbility
    {
        public MagneticFieldAbility(ActiveAbility ability)
            : base(ability)
        {
            this.field = (MagneticField) ability;
        }

        private readonly MagneticField field;


        public override bool CanHit(TargetManager targetManager, IComboModeMenu comboMenu)
        {
            return this.Ability.CanHit(this.Owner);
        }

        public override bool ShouldCast(TargetManager targetManager)
        {
            var abilityOwner = this.Owner;
            var mainHero = EntityManager9.Owner.Hero;

            var enemyTarget = targetManager.Target;


            if (abilityOwner.Distance(enemyTarget) > abilityOwner.GetAttackRange() + 20)
            {
                return false;
            }

            if (abilityOwner.IsInvulnerable)
            {
                return false;
            }

            if (abilityOwner.Equals(this.Shield.Owner))
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

            if (abilityOwner.IsMagicImmune && !this.Shield.PiercesMagicImmunity(abilityOwner))
            {
                return false;
            }

            if (abilityOwner.HasModifier(this.Shield.ShieldModifierName) && mainHero.HealthPercentage > 30)
            {
                return false;
            }

            if (this.Shield is ToggleAbility toggle && toggle.Enabled)
            {
                return false;
            }

            return true;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
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

            if (enemies.Count(x =>
                x.Distance(abilityOwner) < abilityOwner.GetAttackRange() ||
                x.Distance(mainHero) < mainHero.Hero.GetAttackRange()) < 1)
            {
                return false;
            }


            var closestEnemy = enemies.OrderBy(x => x.Distance(abilityOwner)).FirstOrDefault();

            var closestEnemyToMain = enemies.OrderBy(x => x.Distance(mainHero)).FirstOrDefault();


            if (closestEnemyToMain != null && closestEnemyToMain.Distance(mainHero) < 300 &&
                abilityOwner.Distance(mainHero) < 800)
            {
                var position = mainHero.Hero.Position.Extend2D(closestEnemyToMain.Position, -this.field.Radius);

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