﻿namespace O9K.Core.Entities.Units.Unique;

using System;

using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;

using Managers.Entity;

using Metadata;

[UnitName("npc_dota_courier")]
internal class Courier : Unit9
{
    private readonly Divine.Entity.Entities.Units.Courier courier;

    private float hpBarOffset;

    private bool hpBarOffsetUpdated;

    private bool isFlying;

    public Courier(Unit baseUnit)
        : base(baseUnit)
    {
        this.IsCourier = true;
        this.IsUnit = false;
        this.courier = (Divine.Entity.Entities.Units.Courier)baseUnit;
    }

    public bool IsFlying
    {
        get
        {
            if (this.isFlying)
            {
                return true;
            }

            return this.isFlying = this.courier.IsFlying;
        }
    }

    public override Vector3 Position
    {
        get
        {
            if (!this.IsVisible)
            {
                return this.GetPredictedPosition();
            }

            return this.BasePosition;
        }
    }

    internal override float HpBarOffset
    {
        get
        {
            if (this.hpBarOffsetUpdated)
            {
                return this.hpBarOffset;
            }

            if (!this.IsFlying || !this.IsVisible)
            {
                return base.HpBarOffset;
            }

            this.hpBarOffset = this.BaseUnit.HealthBarOffset;
            this.hpBarOffsetUpdated = true;

            return this.hpBarOffset;
        }
    }

    public override Vector3 GetPredictedPosition(float delay = 0, bool forceMovement = false)
    {
        if (!this.IsMoving)
        {
            if (!this.IsVisible)
            {
                return this.BasePosition.Extend(
                    EntityManager9.EnemyFountain,
                    ((GameManager.RawGameTime - this.LastVisibleTime) + delay) * this.Speed);
            }

            return this.BasePosition;
        }

        var rotation = this.BaseUnit.NetworkRotationRad;
        var polar = new Vector3((float)Math.Cos(rotation), (float)Math.Sin(rotation), 0);

        return this.BasePosition + (polar * ((GameManager.RawGameTime - this.LastVisibleTime) + delay) * this.Speed);
    }
}