namespace O9K.AIO.Heroes.ArcWarden.Units
{
    using System;
    using System.Linq;

    using Base;

    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Managers.Entity;

    using Divine.Entity.Entities.Components;
    using Divine.Numerics;
    using Divine.Order;

    using Utils;

    internal interface IPushUnit
    {
        bool PushCombo();

        bool IsValid { get; }
    }

    internal class PushUnit : ControllableUnit, IPushUnit
    {
        private readonly Sleeper moveSleeper = new Sleeper();

        private readonly LaneHelper laneHelper = new LaneHelper();

        public PushUnit(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
        }

        public PushUnit(ControllableUnit unit)
            : base(unit.Owner, unit.abilitySleeper, unit.OrbwalkSleeper, unit.Menu)
        {
        }

        public bool PushCombo()
        {
            if (OrderManager.Orders.Count() != 0)
            {
                return false;
            }

            var nearestTower =
                EntityManager9.EnemyUnits
                    .Where(x => x.BaseUnit.NetworkName == ClassId.CDOTA_BaseNPC_Tower.ToString() && x.IsValid && x.IsAlive)
                    .OrderBy(y => this.Owner.Distance(y))
                    .FirstOrDefault();

            if (nearestTower == null)
            {
                nearestTower = EntityManager9.EnemyUnits.Where(x => x.IsBuilding && x.IsValid && x.IsAlive && x.CanDie).OrderBy(y => this.Owner.Distance(y))
                    .FirstOrDefault();
            }

            var currentLane = laneHelper.GetCurrentLane(this.Owner);
            var attackPoint = laneHelper.GetClosestAttackPoint(this.Owner, currentLane);

            if (this.Owner.Distance(nearestTower) <= 900)
            {
                if (AttackTower(nearestTower))
                {
                    return true;
                }
            }

            Console.WriteLine("???");

            if (AttackNextPoint(attackPoint))
            {
                return true;
            }

            return true;
        }

        private bool AttackNextPoint(Vector3 attackPoint)
        {
            if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWarden.PushCombo.Attack" + this.Owner.Handle))
            {
                this.Owner.BaseUnit.Attack(attackPoint);
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWarden.PushCombo.Attack" + this.Owner.Handle, 1500);

                return true;
            }

            return false;
        }

        private bool AttackTower(Unit9 nearestTower)
        {
            if (!this.Owner.IsAttacking && !Divine.Helpers.MultiSleeper<string>.Sleeping("ArcWarden.PushCombo.Attack" + this.Owner.Handle) && !nearestTower.IsInvulnerable)
            {
                this.Owner.Attack(nearestTower);
                Divine.Helpers.MultiSleeper<string>.Sleep("ArcWarden.PushCombo.Attack" + this.Owner.Handle, 400);

                return true;
            }

            return false;
        }
    }
}