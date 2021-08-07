﻿namespace O9K.AIO.Heroes.ArcWarden.Units
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

    internal partial class PushUnit : ControllableUnit, IPushUnit
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
                if (PushCommands.AttackTower(this.Owner, nearestTower))
                {
                    return true;
                }
            }

            Console.WriteLine("???");

            if (PushCommands.AttackNextPoint(this.Owner, attackPoint))
            {
                return true;
            }

            return true;
        }
    }
}