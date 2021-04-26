namespace O9K.Evader.Abilities.Units.Courier.Shield
{
    using System;

    using Base.Usable.CounterAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Logger;
    using Core.Managers.Entity;

    using Divine;

    using Metadata;

    internal class ShieldUsable : CounterAbility, IDisposable
    {
        public ShieldUsable(Ability9 ability, IMainMenu menu)
            : base(ability, menu)
        {
            EntityManager9.UnitMonitor.AttackStart += this.OnAttackStart;
            ProjectileManager.TrackingProjectileAdded += this.OnAddTrackingProjectile;
        }

        public void Dispose()
        {
            EntityManager9.UnitMonitor.AttackStart -= this.OnAttackStart;
            ProjectileManager.TrackingProjectileAdded -= this.OnAddTrackingProjectile;
        }

        private void OnAddTrackingProjectile(TrackingProjectileAddedEventArgs args)
        {
            if (args.IsCollection)
            {
                return;
            }

            try
            {
                if (!this.Menu.AbilitySettings.IsCounterEnabled(this.Ability.Name))
                {
                    return;
                }

                var projectile = args.TrackingProjectile;
                if (projectile == null || projectile.Source == null)
                {
                    return;
                }

                if (projectile.Target?.Handle != this.Ability.Owner.Handle)
                {
                    return;
                }

                var unit = EntityManager9.GetUnit(projectile.Source.Handle);
                if (unit == null || unit.Team == this.Ability.Owner.Team)
                {
                    return;
                }

                if (this.Ability.CanBeCasted())
                {
                    this.ActiveAbility.UseAbility(false, true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAttackStart(Unit9 unit)
        {
            try
            {
                if (!this.Menu.AbilitySettings.IsCounterEnabled(this.Ability.Name))
                {
                    return;
                }

                if (!unit.IsHero || unit.IsRanged || unit.Team == this.Owner.Team)
                {
                    return;
                }

                if (unit.Target?.Handle != this.Ability.Owner.Handle)
                {
                    return;
                }

                if (this.Ability.CanBeCasted())
                {
                    this.ActiveAbility.UseAbility(false, true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}