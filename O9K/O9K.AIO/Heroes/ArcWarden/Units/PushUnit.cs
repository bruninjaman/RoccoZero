namespace O9K.AIO.Heroes.ArcWarden.Units
{
    using System.Linq;

    using Base;

    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Managers.Entity;

    using Divine.Entity.Entities.Components;
    using Divine.Order;

    using Utils;

    internal interface IPushUnit
    {
        bool IsValid { get; }

        bool PushCombo();
    }

    internal class PushUnit : ControllableUnit, IPushUnit
    {
        private readonly LaneHelper laneHelper = new();

        private readonly Sleeper moveSleeper = new();

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
                    .OrderBy(y => Owner.Distance(y))
                    .FirstOrDefault();

            if (nearestTower == null)
            {
                nearestTower = EntityManager9.EnemyUnits.Where(x => x.IsBuilding && x.IsValid && x.IsAlive && x.CanDie).OrderBy(y => Owner.Distance(y))
                    .FirstOrDefault();
            }

            var currentLane = laneHelper.GetCurrentLane(Owner);
            var attackPoint = laneHelper.GetClosestAttackPoint(Owner, currentLane);

            if (Owner.Distance(nearestTower) <= 900)
            {
                if (PushCommands.AttackTower(Owner, nearestTower))
                {
                    return true;
                }
            }

            if (PushCommands.AttackNextPoint(Owner, attackPoint))
            {
                return true;
            }

            return true;
        }
    }
}