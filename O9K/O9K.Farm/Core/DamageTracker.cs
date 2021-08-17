﻿namespace O9K.Farm.Core
{
    using System;
    using System.Linq;

    using Damage;

    using Divine.Game;
    using Divine.Projectile;
    using Divine.Projectile.EventArgs;

    using O9K.Core.Entities.Units;
    using O9K.Core.Extensions;
    using O9K.Core.Logger;
    using O9K.Core.Managers.Entity;

    internal class DamageTracker : IDisposable
    {
        private readonly UnitManager unitManager;

        public DamageTracker(UnitManager unitManager)
        {
            this.unitManager = unitManager;

            EntityManager9.UnitMonitor.AttackStart += this.OnAttackStart;
            // EntityManager9.UnitMonitor.AttackEnd += this.OnUnitDied;
            EntityManager9.UnitMonitor.UnitDied += this.OnUnitDied;
            ProjectileManager.TrackingProjectileAdded += this.OnAddTrackingProjectile;
            // RendererManager.Draw += this.TrackAddedProjectile;
        }

        public void Dispose()
        {
            EntityManager9.UnitMonitor.AttackStart -= this.OnAttackStart;
            EntityManager9.UnitMonitor.AttackEnd -= this.OnUnitDied;
            EntityManager9.UnitMonitor.UnitDied -= this.OnUnitDied;
            ProjectileManager.TrackingProjectileAdded -= this.OnAddTrackingProjectile;
        }

        // Draw projectile position
        // private void TrackAddedProjectile()
        // {
        //     var unitManagerUnits = this.unitManager.Units;
        //
        //     if (unitManagerUnits != null)
        //     {
        //         var allUnits = unitManagerUnits.ToList();
        //
        //         var rangedDamages = allUnits.SelectMany(x => x.IncomingDamage).OfType<RangedDamage>().ToList();
        //
        //         foreach (var rangedDamage in rangedDamages.Where(x => x.Projectile is not null && x.Projectile.IsValid))
        //         {
        //             // Console.WriteLine(rangedDamage.Source.Unit.Name);
        //             var projectile = rangedDamage.Projectile;
        //
        //             if (projectile.Source is not null)
        //             {
        //                 var sourceScreenPos = RendererManager.WorldToScreen(projectile.Source.Position);
        //                 var projectilePosition = RendererManager.WorldToScreen(projectile.Position);
        //
        //                 var rectangleF = new RectangleF(projectilePosition.X - 10, projectilePosition.Y - 10, 20, 20);
        //                 RendererManager.DrawFilledRectangle(rectangleF, Color.Red);
        //
        //             }
        //         }
        //
        //     }
        // }

        public event EventHandler<UnitDamage> AttackCanceled;

        private void OnAddTrackingProjectile(TrackingProjectileAddedEventArgs e)
        {
            try
            {
                var projectile = e.Projectile;

                if (!projectile.IsAutoAttackProjectile())
                {
                    return;
                }

                var source = this.unitManager.GetUnit(projectile.Source);

                if (source?.IsControllable != false)
                {
                    return;
                }

                var target = projectile.Target;

                if (target == null)
                {
                    return;
                }

                var allUnits = this.unitManager.Units.ToList();
                var damages = allUnits.SelectMany(x => x.IncomingDamage).OfType<RangedDamage>().ToList();

                var damage = damages.LastOrDefault(
                    x => x.Projectile == null && !x.IsPredicted && x.MinDamage > 0 && x.Source.Unit.Handle == source.Unit.Handle
                         && x.Target.Unit.Handle == target.Handle);

                if (damage != null)
                {
                    if (GameManager.RawGameTime < damage.HitTime)
                    {
                        damage.AddProjectile(projectile);
                    }
                }
                else
                {
                    damage = damages.Find(x => x.Projectile == null && x.Source.Unit.Handle == source.Unit.Handle);

                    if (damage != null)
                    {
                        //wrong projectile target
                        damage.Delete();
                        this.AttackCanceled?.Invoke(this, damage);
                    }

                    var newTarget = allUnits.Find(x => x.Unit.Handle == target.Handle);

                    if (newTarget == null)
                    {
                        return;
                    }

                    damage = (RangedDamage)source.AddDamage(newTarget, 0, false, true);
                    damage.AddProjectile(projectile);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnAttackStart(Unit9 unit)
        {
            try
            {
                var units = this.unitManager.Units.ToList();
                var farmUnit = units.Find(x => x.Unit == unit);

                if (farmUnit == null)
                {
                    return;
                }

                var time = GameManager.RawGameTime - GameManager.Ping / 2000;

                if (farmUnit.IsControllable)
                {
                    farmUnit.AttackStartTime = time;

                    return;
                }

                farmUnit.AttackStart(units, time);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUnitDied(Unit9 unit)
        {
            try
            {
                foreach (var farmUnit in this.unitManager.Units)
                {
                    foreach (var damage in farmUnit.IncomingDamage)
                    {
                        if (damage.MinDamage <= 0 || damage.Source.Unit != unit || damage.HitTime <= GameManager.RawGameTime)
                        {
                            continue;
                        }

                        if (damage is RangedDamage ranged && ranged.Projectile != null)
                        {
                            continue;
                        }

                        damage.Delete();
                        this.AttackCanceled?.Invoke(this, damage);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}