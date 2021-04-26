namespace O9K.Evader.ActionManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities;
    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Heroes.Morphling;
    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Menu.Items;

    using Divine;

    using Metadata;

    using Pathfinder.Obstacles;

    internal class ActionManager : IEvaderService, IActionManager
    {
        private readonly MultiSleeper abilityInputBlocked = new MultiSleeper();

        private readonly Sleeper actionCheck = new Sleeper();

        private readonly HashSet<OrderType> ignoredActions = new HashSet<OrderType>
        {
            OrderType.UpgradeSpell,
            OrderType.CastToggleAutocast,
            OrderType.BuyItem,
            OrderType.DisassembleItem,
            OrderType.SetItemCombiningLock,
            OrderType.SellItem,
            OrderType.MoveItem,
            OrderType.DropFromStash,
            OrderType.Announce,
            OrderType.Glyph,
            OrderType.Scan
        };

        private readonly Dictionary<Unit9, MultiSleeper> ignoredModifierObstacles = new Dictionary<Unit9, MultiSleeper>();

        private readonly MultiSleeper ignoredObstacles = new MultiSleeper();

        private readonly Dictionary<Unit9, MultiSleeper> ignoredUnitObstacles = new Dictionary<Unit9, MultiSleeper>();

        private readonly Dictionary<Entity9, MultiSleeper> inputObstacleBlocked = new Dictionary<Entity9, MultiSleeper>();

        private readonly MenuHoldKey overrideKey;

        private readonly MultiSleeper unitInputBlocked = new MultiSleeper();

        private Ability9 cancelChannelingAbility;

        private UpdateHandler stopHandler;

        public ActionManager(IMainMenu menu)
        {
            this.overrideKey = menu.Hotkeys.OverrideDodgeMode;
        }

        public LoadOrder LoadOrder { get; } = LoadOrder.ActionManager;

        private UpdateHandler StopHandler
        {
            get
            {
                if (this.stopHandler == null)
                {
                    this.stopHandler = UpdateManager.CreateIngameUpdate(0, false, this.StopChanneling);
                }

                return this.stopHandler;
            }
        }

        public void Activate()
        {
            OrderManager.OrderAdding += OnOrderAdding;
        }

        public void BlockInput(Unit9 unit, float seconds)
        {
            this.unitInputBlocked.Sleep(unit.Handle, seconds);
            this.actionCheck.ExtendSleep(seconds);
        }

        public void BlockInput(Ability9 ability, IObstacle obstacle, float seconds)
        {
            if (ability is AttributeShiftStrengthGain shift)
            {
                seconds += 0.5f;
                this.BlockInput(shift.AttributeShiftAgilityGain, obstacle, seconds);
            }

            if (!this.inputObstacleBlocked.TryGetValue(ability, out var obstacleInput))
            {
                this.inputObstacleBlocked[ability] = obstacleInput = new MultiSleeper();
            }

            obstacleInput.Sleep(obstacle.Id, seconds);
            this.abilityInputBlocked.Sleep(ability.Handle, seconds);
            this.actionCheck.ExtendSleep(seconds);
        }

        public void BlockInput(Unit9 unit, IObstacle obstacle, float seconds)
        {
            if (!this.inputObstacleBlocked.TryGetValue(unit, out var obstacleInput))
            {
                this.inputObstacleBlocked[unit] = obstacleInput = new MultiSleeper();
            }

            obstacleInput.Sleep(obstacle.Id, seconds);
            this.unitInputBlocked.Sleep(unit.Handle, seconds);
            this.actionCheck.ExtendSleep(seconds);
        }

        public void CancelChanneling(Ability9 ability, float timeout = 2)
        {
            this.cancelChannelingAbility = ability;
            this.StopHandler.IsEnabled = true;
            UpdateManager.BeginInvoke((int)(timeout * 1000), () => this.stopHandler.IsEnabled = false);
        }

        public void Dispose()
        {
            OrderManager.OrderAdding -= OnOrderAdding;

            if (this.stopHandler != null)
            {
                UpdateManager.DestroyIngameUpdate(this.stopHandler);
            }
        }

        public IEnumerable<Unit9> GetEvadingObstacleUnits(IObstacle obstacle)
        {
            return this.ignoredUnitObstacles.Where(x => x.Value.IsSleeping(obstacle.Id)).Select(x => x.Key);
        }

        public void IgnoreModifierObstacle(uint abilityHandle, Unit9 unit, float seconds)
        {
            if (!this.ignoredModifierObstacles.TryGetValue(unit, out var modifierObstacleSleeper))
            {
                modifierObstacleSleeper = new MultiSleeper();
                this.ignoredModifierObstacles[unit] = modifierObstacleSleeper;
            }

            modifierObstacleSleeper.Sleep(abilityHandle, seconds);
        }

        public void IgnoreObstacle(Unit9 unit, IObstacle obstacle, float seconds)
        {
            if (!this.ignoredUnitObstacles.TryGetValue(unit, out var sleeper))
            {
                sleeper = new MultiSleeper();
                this.ignoredUnitObstacles[unit] = sleeper;
            }

            sleeper.Sleep(obstacle.Id, seconds);
        }

        public void IgnoreObstacle(IObstacle obstacle, float seconds)
        {
            this.ignoredObstacles.Sleep(obstacle.Id, seconds);
        }

        public bool IsInputBlocked(Unit9 unit)
        {
            return this.unitInputBlocked.IsSleeping(unit.Handle);
        }

        public bool IsObstacleIgnored(Unit9 unit, IObstacle obstacle)
        {
            if (this.ignoredObstacles.IsSleeping(obstacle.Id))
            {
                return true;
            }

            if (this.ignoredUnitObstacles.TryGetValue(unit, out var sleeper) && sleeper.IsSleeping(obstacle.Id))
            {
                return true;
            }

            if (obstacle.IsModifierObstacle)
            {
                return this.ignoredModifierObstacles.TryGetValue(unit, out sleeper)
                       && sleeper.IsSleeping(obstacle.EvadableAbility.Ability.Handle);
            }

            if (unit.IsLinkensProtected || unit.IsSpellShieldProtected)
            {
                var ability = obstacle.EvadableAbility.ActiveAbility;
                if (ability?.BreaksLinkens == true && (!unit.IsSpellShieldProtected || ability.Id != AbilityId.legion_commander_duel))
                {
                    return true;
                }
            }

            return false;
        }

        public void UnblockInput(IObstacle obstacle)
        {
            foreach (var entity in this.inputObstacleBlocked.Where(x => x.Value.IsSleeping(obstacle.Id)).Select(x => x.Key))
            {
                if (entity is Unit9 unit)
                {
                    if (!unit.IsInvulnerable)
                    {
                        unit.BaseUnit.Stop(false, true);
                    }

                    this.unitInputBlocked.Reset(entity.Handle);
                }
                else if (entity is Ability9 ability)
                {
                    if (ability is ToggleAbility toggle && toggle.Enabled && ability.CanBeCasted())
                    {
                        toggle.UseAbility(false, true);
                    }

                    this.abilityInputBlocked.Reset(entity.Handle);
                }

                this.inputObstacleBlocked[entity].Reset(obstacle.Id);
            }
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                var order = e.Order;
                var orderType = e.Order.Type;
                if (!this.actionCheck.IsSleeping || this.ignoredActions.Contains(orderType))
                {
                    return;
                }

                if (order.Type == OrderType.CastToggle)
                {
                    if (this.abilityInputBlocked.IsSleeping(order.Ability.Handle))
                    {
                        e.Process = false;
                    }

                    return;
                }

                if (this.overrideKey.IsActive)
                {
                    return;
                }

                if (order.Units.Any(x => this.unitInputBlocked.IsSleeping(x.Handle)))
                {
                    e.Process = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void StopChanneling()
        {
            try
            {
                if (!this.cancelChannelingAbility.IsValid || !this.cancelChannelingAbility.BaseAbility.IsChanneling)
                {
                    return;
                }

                var owner = this.cancelChannelingAbility.Owner;
                if (!owner.IsValid || !owner.IsAlive)
                {
                    return;
                }

                owner.BaseUnit.Stop(false, true);
                this.stopHandler.IsEnabled = false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}