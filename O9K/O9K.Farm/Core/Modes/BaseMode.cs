﻿namespace O9K.Farm.Core.Modes;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Entity.Entities;
using Divine.Entity.Entities.Players;
using Divine.Extensions;
using Divine.Game;
using Divine.Update;

using O9K.Core.Entities.Units;
using O9K.Core.Helpers;
using O9K.Core.Logger;

using Units.Base;

internal abstract class BaseMode : IDisposable
{
    private readonly Sleeper actionSleeper = new();

    private readonly UpdateHandler handler;

    private readonly List<FarmUnit> units = new();

    protected BaseMode(UnitManager unitManager)
    {
        this.UnitManager = unitManager;
        this.handler = UpdateManager.CreateIngameUpdate(0, false, this.OnUpdate);
    }

    public bool IsActive
    {
        get
        {
            return this.handler.IsEnabled;
        }
    }
    
    public List<FarmUnit> LastAddedUnits { get; protected set; } = new();

    protected UnitManager UnitManager { get; }

    public void Dispose()
    {
        UpdateManager.DestroyIngameUpdate(this.handler);
    }

    public void AddUnits(IEnumerable<FarmUnit> farmUnits)
    {
        this.LastAddedUnits = farmUnits.ToList();
        this.units.AddRange(this.LastAddedUnits.Except(this.units));
        this.handler.IsEnabled = true;
    }

    public void AttackCanceled(FarmUnit target)
    {

        var myFarmUnits = this.units.Where(x => x.IsControllable);

        List<FarmUnit> myFarmUnitToStop = new();
        float damages = 0;
        float delay = 0;

        foreach (var unit in myFarmUnits)
        {
            if (unit.Target?.Equals(target) != true)
            {
                return;
            }

            if (!unit.AttackSleeper.IsSleeping)
            {
                return;
            }

            var newDelay = unit.AttackStartTime + unit.GetSimpleAttackDelay(target) - GameManager.RawGameTime;
            delay = delay > newDelay ? delay : newDelay;
            damages += unit.GetDamage(target);
            myFarmUnitToStop.Add(unit);
        }

        if (myFarmUnitToStop.Count != 0 && target.GetPredictedHealth(delay) > damages)
        {
            myFarmUnitToStop.ForEach(x => x.Stop());
        }
    }

    public bool ContainsAllUnits(IEnumerable<FarmUnit> myUnits)
    {
        return myUnits.All(x => this.units.Contains(x));
    }

    public void RemoveLastAddedUnits()
    {
        this.RemoveUnits(this.LastAddedUnits);
    }

    public void RemoveUnit(Unit9 unit)
    {
        if (!this.handler.IsEnabled)
        {
            return;
        }

        var farmUnit = this.units.Find(x => x.Unit == unit);

        if (farmUnit == null)
        {
            return;
        }

        this.units.Remove(farmUnit);

        if (this.units.Any(x => x.Unit.IsValid && x.Unit.IsAlive))
        {
            return;
        }

        this.units.Clear();
        this.handler.IsEnabled = false;
    }

    public void RemoveUnits(IEnumerable<Entity> entities)
    {
        if (!this.handler.IsEnabled)
        {
            return;
        }

        this.RemoveUnits(this.units.Where(x => entities.Contains(x.Unit.BaseUnit)));
    }

    public void RemoveUnits(IEnumerable<FarmUnit> farmUnits)
    {
        this.units.RemoveAll(farmUnits.Contains);

        if (this.units.Any(x => x.Unit.IsValid && x.Unit.IsAlive))
        {
            return;
        }

        this.units.Clear();
        this.handler.IsEnabled = false;
    }

    protected void MoveToMouse(IEnumerable<FarmUnit> myUnits)
    {
        if (this.actionSleeper.IsSleeping)
        {
            return;
        }

        var mousePosition = GameManager.MousePosition;
        var control = myUnits.Where(x => x.CanMoveToMouse() && x.LastMovePosition.Distance2D(mousePosition) > 50).ToList();

        if (control.Count == 0)
        {
            return;
        }

        if (!Player.Move(control.Select(x => x.Unit.BaseUnit), mousePosition))
        {
            return;
        }

        foreach (var unit in control)
        {
            unit.LastMovePosition = mousePosition;
            unit.Target = null;
        }

        this.actionSleeper.Sleep(0.2f);
    }

    protected abstract void OnUpdate(IReadOnlyList<FarmUnit> units, IReadOnlyList<FarmUnit> myUnits);

    private void OnUpdate()
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        try
        {
            this.OnUpdate(this.UnitManager.Units.ToList(), this.units.Where(x => x.IsValid).ToList());
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}