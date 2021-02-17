namespace O9K.Farm.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Damage;

    using Divine;

    using Marker;

    using Menu;

    using Modes;

    using O9K.Core.Entities.Heroes;
    using O9K.Core.Entities.Units;
    using O9K.Core.Logger;
    using O9K.Core.Managers.Entity;

    using Units.Base;

    using Utils;

    internal class FarmManager : IDisposable
    {
        private readonly Dictionary<uint, ControlEffect> controlEffects = new Dictionary<uint, ControlEffect>();

        private readonly DamageTracker damageTracker;

        private readonly List<BaseMode> farmModes = new List<BaseMode>();

        private readonly LastHitMarker lastHitMarker;

        private readonly BaseMode lastHitMode;

        private readonly MenuManager menuManager;

        private readonly Owner owner;

        private readonly BaseMode pushMode;

        private readonly HashSet<OrderType> stopFarmOrders = new HashSet<OrderType>
        {
            OrderType.Hold,
            OrderType.Continue,
            OrderType.Stop,
            OrderType.MovePosition,
            OrderType.MoveTarget,
            OrderType.MoveToDirection,
            OrderType.AttackPosition,
            OrderType.AttackTarget,
            OrderType.Cast,
            OrderType.CastPosition,
            OrderType.CastTarget,
            OrderType.CastRune,
            OrderType.CastTree
        };

        private readonly UnitManager unitManager;

        public FarmManager(MenuManager menuManager)
        {
            this.owner = EntityManager9.Owner;
            this.menuManager = menuManager;
            this.unitManager = new UnitManager(this.menuManager);
            this.damageTracker = new DamageTracker(this.unitManager);
            this.lastHitMarker = new LastHitMarker(this.unitManager, menuManager);

            this.farmModes.Add(this.lastHitMode = new LastHitMode(this.unitManager, menuManager));
            this.farmModes.Add(this.pushMode = new PushMode(this.unitManager, menuManager));

            this.menuManager.LastHitMenu.HoldKey.ValueChange += this.LastHitHoldKeyOnValueChange;
            this.menuManager.LastHitMenu.ToggleKey.ValueChange += this.LastHitToggleKeyOnValueChange;

            //  this.menuManager.PushMenu.HoldKey.ValueChange += this.PushHoldKeyOnValueChange;
            //  this.menuManager.PushMenu.ToggleKey.ValueChange += this.PushToggleKeyOnValueChange;

            this.damageTracker.AttackCanceled += this.OnAttackCanceled;
            EntityManager9.UnitMonitor.UnitDied += this.OnUnitDied;
            EntityManager9.UnitRemoved += this.OnUnitDied;
            OrderManager.OrderAdding += this.OrderAdding;
        }

        public void Dispose()
        {
            OrderManager.OrderAdding -= this.OrderAdding;
            EntityManager9.UnitMonitor.UnitDied -= this.OnUnitDied;
            EntityManager9.UnitRemoved -= this.OnUnitDied;
            this.damageTracker.AttackCanceled -= this.OnAttackCanceled;

            this.menuManager.LastHitMenu.HoldKey.ValueChange -= this.LastHitHoldKeyOnValueChange;
            this.menuManager.LastHitMenu.ToggleKey.ValueChange -= this.LastHitToggleKeyOnValueChange;

            //    this.menuManager.PushMenu.HoldKey.ValueChange -= this.PushHoldKeyOnValueChange;
            //    this.menuManager.PushMenu.ToggleKey.ValueChange -= this.PushToggleKeyOnValueChange;

            this.lastHitMode.Dispose();
            this.damageTracker.Dispose();
            this.lastHitMarker.Dispose();
            this.unitManager.Dispose();
        }

        private void AddEffects(IEnumerable<FarmUnit> units)
        {
            foreach (var farmUnit in units)
            {
                if (this.controlEffects.ContainsKey(farmUnit.Unit.Handle))
                {
                    continue;
                }

                this.controlEffects[farmUnit.Unit.Handle] = new ControlEffect(farmUnit);
            }
        }

        private void LastHitHoldKeyOnValueChange(object sender, O9K.Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            try
            {
                if (e.NewValue)
                {
                    var units = this.owner.SelectedUnits.Select(x => this.unitManager.GetControllableUnit(x))
                        .Where(x => x != null)
                        .ToList();

                    foreach (var farmMode in this.farmModes)
                    {
                        farmMode.RemoveUnits(units);
                    }

                    this.AddEffects(units);
                    this.lastHitMode.AddUnits(units);
                }
                else
                {
                    this.RemoveEffects(this.lastHitMode.LastAddedUnits);
                    this.lastHitMode.RemoveLastAddedUnits();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void LastHitToggleKeyOnValueChange(object sender, O9K.Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            try
            {
                if (!e.NewValue)
                {
                    return;
                }

                var units = this.owner.SelectedUnits.Select(x => this.unitManager.GetControllableUnit(x)).Where(x => x != null).ToList();

                var disable = this.lastHitMode.ContainsAllUnits(units);

                foreach (var farmMode in this.farmModes)
                {
                    farmMode.RemoveUnits(units);
                }

                if (disable)
                {
                    this.RemoveEffects(this.lastHitMode.LastAddedUnits);
                    this.lastHitMode.RemoveLastAddedUnits();
                }
                else
                {
                    this.AddEffects(units);
                    this.lastHitMode.AddUnits(units);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void OnAttackCanceled(object sender, UnitDamage damage)
        {
            foreach (var farmMode in this.farmModes)
            {
                farmMode.AttackCanceled(damage.Target);
            }
        }

        private void OrderAdding(OrderAddingEventArgs e)
        {
            if (this.menuManager.LastHitMenu.HoldKey)
            {
                return;
            }

            var order = e.Order;
            if (!e.Process || order.IsQueued || e.IsCustom || !this.stopFarmOrders.Contains(order.Type))
            {
                return;
            }

            foreach (var farmMode in this.farmModes)
            {
                this.RemoveEffects(order.Units);
                farmMode.RemoveUnits(order.Units);
            }
        }

        private void OnUnitDied(Unit9 unit)
        {
            try
            {
                foreach (var farmMode in this.farmModes)
                {
                    farmMode.RemoveUnit(unit);
                }

                if (this.controlEffects.TryGetValue(unit.Handle, out var effect))
                {
                    effect.Dispose();
                    this.controlEffects.Remove(unit.Handle);
                }

                if (this.farmModes.Any(x => x.IsActive))
                {
                    return;
                }

                //todo disable all shit
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void PushHoldKeyOnValueChange(object sender, O9K.Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            try
            {
                if (e.NewValue)
                {
                    var units = this.owner.SelectedUnits.Select(x => this.unitManager.GetControllableUnit(x))
                        .Where(x => x != null)
                        .ToList();

                    foreach (var farmMode in this.farmModes)
                    {
                        farmMode.RemoveUnits(units);
                    }

                    this.AddEffects(units);
                    this.pushMode.AddUnits(units);
                }
                else
                {
                    this.RemoveEffects(this.pushMode.LastAddedUnits);
                    this.pushMode.RemoveLastAddedUnits();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void PushToggleKeyOnValueChange(object sender, O9K.Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            try
            {
                if (!e.NewValue)
                {
                    return;
                }

                var units = this.owner.SelectedUnits.Select(x => this.unitManager.GetControllableUnit(x)).Where(x => x != null).ToList();

                var disable = this.pushMode.ContainsAllUnits(units);

                foreach (var farmMode in this.farmModes)
                {
                    farmMode.RemoveUnits(units);
                }

                if (disable)
                {
                    this.RemoveEffects(this.pushMode.LastAddedUnits);
                    this.pushMode.RemoveLastAddedUnits();
                }
                else
                {
                    this.AddEffects(units);
                    this.pushMode.AddUnits(units);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        private void RemoveEffects(IEnumerable<Entity> entities)
        {
            foreach (var farmUnit in entities)
            {
                if (!this.controlEffects.TryGetValue(farmUnit.Handle, out var effect))
                {
                    continue;
                }

                effect.Dispose();
                this.controlEffects.Remove(farmUnit.Handle);
            }
        }

        private void RemoveEffects(IEnumerable<FarmUnit> units)
        {
            foreach (var farmUnit in units)
            {
                if (!this.controlEffects.TryGetValue(farmUnit.Unit.Handle, out var effect))
                {
                    continue;
                }

                effect.Dispose();
                this.controlEffects.Remove(farmUnit.Unit.Handle);
            }
        }
    }
}