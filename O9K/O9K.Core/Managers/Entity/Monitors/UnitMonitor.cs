namespace O9K.Core.Managers.Entity.Monitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Divine;

    using Entities.Abilities.Base.Components;
    using Entities.Abilities.Base.Types;
    using Entities.Heroes;
    using Entities.Heroes.Unique;
    using Entities.Units;

    using Helpers;
    using Helpers.Damage;
    using Helpers.Range;

    using Logger;

    public sealed class UnitMonitor : IDisposable
    {
        private readonly Team allyTeam;

        private readonly HashSet<int> attackActivities = new HashSet<int>
        {
            (int)NetworkActivity.Attack,
            (int)NetworkActivity.Attack2,
            (int)NetworkActivity.Crit,
            (int)NetworkActivity.AttackEvent,
            (int)NetworkActivity.AttackEventBash
        };

        private readonly HashSet<string> attackAnimation = new HashSet<string>
        {
            "tidebringer",
            "impetus_anim"
        };

        private readonly MultiSleeper attackSleeper = new MultiSleeper();

        private readonly DamageFactory damageFactory;

        private readonly HashSet<string> notAttackAnimation = new HashSet<string>
        {
            "sniper_attack_schrapnel_cast1_aggressive",
            "sniper_attack_schrapnel_cast1_aggressive_anim",
            "attack_omni_cast",
            "lotfl_dualwield_press_the_attack",
            "lotfl_press_the_attack",
            "sniper_attack_assassinate_dreamleague",
        };

        private readonly RangeFactory rangeFactory;

        private readonly Dictionary<string, Action<Unit9, Modifier, bool>> specialModifiers =
            new Dictionary<string, Action<Unit9, Modifier, bool>>
            {
                { "modifier_teleporting", (x, _, value) => x.IsTeleporting = value },
                { "modifier_riki_permanent_invisibility", (u, _, value) => u.CanUseAbilitiesInInvisibility = value },
                { "modifier_ice_blast", (x, _, value) => x.CanBeHealed = !value },
                { "modifier_item_aegis", (x, _, value) => x.HasAegis = value },
                { "modifier_necrolyte_sadist_active", (x, _, value) => x.IsEthereal = value },
                { "modifier_pugna_decrepify", (x, _, value) => x.IsEthereal = value },
                { "modifier_item_ethereal_blade_ethereal", (x, _, value) => x.IsEthereal = value },
                { "modifier_ghost_state", (x, _, value) => x.IsEthereal = value },
                { "modifier_item_lotus_orb_active", (x, _, value) => x.IsLotusProtected = value },
                { "modifier_antimage_counterspell", (x, _, value) => x.IsSpellShieldProtected = value },
                { "modifier_item_sphere_target", (x, _, value) => x.IsLinkensTargetProtected = value },
                { "modifier_item_blade_mail_reflect", (x, _, value) => x.IsReflectingDamage = value },
                { "modifier_item_ultimate_scepter_consumed", (x, _, __) => x.HasAghanimsScepterBlessing = true },
                { "modifier_slark_dark_pact", (x, _, value) => x.IsDarkPactProtected = true },
                { "modifier_slark_dark_pact_pulses", (x, _, value) => x.IsDarkPactProtected = value },
                { "modifier_bloodseeker_rupture", (x, _, value) => x.IsRuptured = value },
                { "modifier_spirit_breaker_charge_of_darkness", (x, _, value) => x.IsCharging = value },
                { "modifier_dragon_knight_dragon_form", (x, _, value) => x.IsRanged = value || x.BaseUnit.IsRanged },
                { "modifier_terrorblade_metamorphosis", (x, _, value) => x.IsRanged = value || x.BaseUnit.IsRanged },
                { "modifier_troll_warlord_berserkers_rage", (x, _, value) => x.IsRanged = !value || x.BaseUnit.IsRanged },
                { "modifier_lone_druid_true_form", (x, _, value) => x.IsRanged = !value || x.BaseUnit.IsRanged },
                {
                    "modifier_brewmaster_primal_split", (x, mod, value) =>
                        {
                            if (value)
                            {
                                x.HideHud = true;
                                x.ForceUnitState(UnitState.Unselectable | UnitState.CommandRestricted, mod.RemainingTime);
                            }
                            else
                            {
                                x.HideHud = false;
                            }
                        }
                },
                {
                    "modifier_slark_shadow_dance_visual", (x, _, value) =>
                        {
                            var slark = x.Owner as Slark;
                            slark?.ShadowDanced(value);
                        }
                },
                {
                    "modifier_morphling_replicate", (x, _, value) =>
                        {
                            var morphling = x as Morphling;
                            morphling?.Morphed(value);
                        }
                },
                {
                    "modifier_alchemist_chemical_rage", (x, _, value) =>
                        {
                            var alchemist = x as Alchemist;
                            alchemist?.Raged(value);
                        }
                }
            };

        public UnitMonitor()
        {
            this.allyTeam = EntityManager9.Owner.Team;
            this.damageFactory = new DamageFactory();
            this.rangeFactory = new RangeFactory();

            Entity.NetworkPropertyChanged += this.OnNetworkPropertyChanged;
            Entity.AnimationChanged += this.OnAnimationChanged;
            ModifierManager.ModifierAdded += this.OnModifierAdded;
            ModifierManager.ModifierRemoved += this.OnModifierRemoved;
            GameManager.GameEvent += this.OnGameEvent;
            OrderManager.OrderAdding += this.OnOrderAdding;

            UpdateManager.CreateUpdate(OnUpdate);
        }

        public delegate void EventHandler(Unit9 unit);

        public delegate void HealthEventHandler(Unit9 unit, float health);

        public event EventHandler AttackEnd;

        public event EventHandler AttackStart;

        public event EventHandler UnitDied;

        public event HealthEventHandler UnitHealthChange;

        public void Dispose()
        {
            this.damageFactory.Dispose();
            this.rangeFactory.Dispose();

            Entity.NetworkPropertyChanged -= this.OnNetworkPropertyChanged;
            Entity.AnimationChanged -= this.OnAnimationChanged;
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
            GameManager.GameEvent -= this.OnGameEvent;
            OrderManager.OrderAdding -= this.OnOrderAdding;

            UpdateManager.DestroyIngameUpdate(OnUpdate);
        }

        internal void CheckModifiers(Unit9 unit)
        {
            try
            {
                foreach (var modifier in unit.BaseModifiers)
                {
                    this.CheckModifier(unit.Handle, modifier, true);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static void DropTarget(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                var unit = EntityManager9.GetUnitFast(entity.Handle);
                if (unit != null)
                {
                    unit.Target = null;
                    unit.IsAttacking = false;
                }
            }
        }

        private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "m_bReincarnating":
                    {
                        var newValue = e.NewValue.GetBoolean();
                        if (newValue == e.OldValue.GetBoolean())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            var unit = (Hero9)EntityManager9.GetUnitFast(sender.Handle);
                            if (unit == null)
                            {
                                return;
                            }

                            unit.IsReincarnating = newValue;
                        });
                    }
                    break;
                case "m_iIsControllableByPlayer64":
                    {
                        if (e.NewValue.GetInt64() == e.OldValue.GetInt64())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            EntityManager9.ChangeEntityControl(sender);
                        });
                    }
                    break;
                case "m_hKillCamUnit":
                    {
                        if (e.NewValue.GetUInt32() == e.OldValue.GetUInt32())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            if (sender.Team == this.allyTeam)
                            {
                                return;
                            }

                            var entity = e.NewValue.GetEntity();
                            if (entity == null || !entity.IsValid)
                            {
                                return;
                            }

                            var handle = ((Player)sender).Hero?.Handle;
                            var unit = (Hero9)EntityManager9.GetUnitFast(handle);
                            if (unit == null)
                            {
                                return;
                            }

                            UpdateManager.BeginInvoke(500, () => RespawnUnit(unit));
                        });
                    }
                    break;
                case "m_iHealth":
                    {
                        var newValue = e.NewValue.GetInt32();
                        if (newValue == e.OldValue.GetInt32())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            var unit = EntityManager9.GetUnitFast(sender.Handle);
                            if (unit == null)
                            {
                                return;
                            }

                            if (newValue > 0)
                            {
                                this.UnitHealthChange?.Invoke(unit, newValue);
                            }
                            else
                            {
                                unit.DeathTime = GameManager.RawGameTime;

                                this.AttackStopped(unit);
                                this.attackSleeper.Remove(unit.Handle);

                                this.UnitDied?.Invoke(unit);
                            }
                        });
                    }
                    break;
                case "m_NetworkActivity":
                    {
                        var newValue = e.NewValue.GetInt32();
                        if (newValue == e.OldValue.GetInt32())
                        {
                            break;
                        }

                        UpdateManager.BeginInvoke(() =>
                        {
                            var unit = EntityManager9.GetUnitFast(sender.Handle);
                            if (unit == null)
                            {
                                return;
                            }

                            if (this.attackActivities.Contains(newValue))
                            {
                                this.AttackStarted(unit);
                            }
                            else
                            {
                                this.AttackStopped(unit);
                            }
                        });
                    }
                    break;
            }
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            var order = e.Order;
            if (order.IsQueued || !e.Process)
            {
                return;
            }

            try
            {
                switch (order.Type)
                {
                    case OrderType.AttackTarget:
                    {
                        SetTarget(order.Units, order.Target.Handle);
                        break;
                    }

                    case OrderType.Hold:
                    case OrderType.Stop:
                    {
                        var units = order.Units;
                        DropTarget(units);
                        StopChanneling(units);
                        break;
                    }

                    case OrderType.MovePosition:
                    case OrderType.MoveTarget:
                    {
                        DropTarget(order.Units);
                        break;
                    }

                    case OrderType.CastTarget:
                    {
                        var target = EntityManager9.GetUnitFast(order.Target.Handle);
                        if (target?.IsLinkensProtected == true)
                        {
                            return;
                        }

                        StartChanneling(order.Ability.Handle);
                        break;
                    }

                    case OrderType.Cast:
                    case OrderType.CastPosition:
                    {
                        StartChanneling(order.Ability.Handle);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private static void OnUpdate()
        {
            foreach (var ability in EntityManager9.abilitiesList)
            {
                try
                {
                    if (!ability.IsValid)
                    {
                        continue;
                    }

                    ability.IsValid = ability.BaseAbility.IsValid;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            foreach (var unit in EntityManager9.unitsList)
            {
                try
                {
                    if (!unit.IsValid)
                    {
                        continue;
                    }

                    if (unit.IsBuilding)
                    {
                        continue;
                    }

                    if (!(unit.IsVisible = unit.BaseUnit.IsVisible))
                    {
                        continue;
                    }

                    unit.LastPositionUpdateTime = GameManager.RawGameTime;
                    unit.BasePosition = unit.BaseUnit.Position;

                    if (!(unit.IsValid = unit.BaseUnit.IsValid))
                    {
                        continue;
                    }

                    if (!unit.IsVisible)
                    {
                        unit.LastNotVisibleTime = GameManager.RawGameTime;
                        continue;
                    }

                    unit.LastVisibleTime = GameManager.RawGameTime;

                    var baseUnit = unit.BaseUnit;

                    if (!(unit.BaseIsAlive = baseUnit.IsAlive))
                    {
                        continue;
                    }

                    unit.BaseHealth = baseUnit.Health;

                    if (unit.IsBuilding)
                    {
                        continue;
                    }

                    unit.BaseMana = baseUnit.Mana;

                    try
                    {
                        unit.Speed = baseUnit.MovementSpeed;
                    }
                    catch (DivideByZeroException)
                    {
                        //hack ensage core bug
                        unit.Speed = 0;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }

        private static void RespawnUnit(Unit9 unit)
        {
            try
            {
                if (!unit.IsValid || unit.IsVisible || unit.BaseIsAlive)
                {
                    return;
                }

                unit.BaseIsAlive = true;
                unit.BaseHealth = unit.MaximumHealth;
                unit.BaseMana = unit.MaximumMana;
                unit.BasePosition = EntityManager9.EnemyFountain;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static void SetTarget(IEnumerable<Entity> entities, uint targetHandle)
        {
            var target = EntityManager9.GetUnit(targetHandle);
            if (target?.IsAlive != true)
            {
                return;
            }

            foreach (var entity in entities)
            {
                var unit = EntityManager9.GetUnitFast(entity.Handle);
                if (unit == null)
                {
                    continue;
                }

                unit.Target = target;
                unit.IsAttacking = true;
            }
        }

        private static void StartChanneling(uint abilityHandle)
        {
            if (!(EntityManager9.GetAbilityFast(abilityHandle) is IChanneled channeled))
            {
                return;
            }

            channeled.Owner.ChannelEndTime = GameManager.RawGameTime + channeled.GetCastDelay() + 1f;
        }

        private static void StopChanneling(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                var unit = EntityManager9.GetUnitFast(entity.Handle);
                if (unit == null)
                {
                    continue;
                }

                unit.ChannelEndTime = 0;
                unit.ChannelActivatesOnCast = false;
            }
        }

        private void AttackStarted(Unit9 unit)
        {
            if (this.attackSleeper.IsSleeping(unit.Handle) || !unit.IsAlive)
            {
                return;
            }

            if (!unit.IsControllable && unit.IsHero)
            {
                unit.Target = EntityManager9.Units
                    .Where(x => x.IsAlive && !x.IsAlly(unit) && x.Distance(unit) <= unit.GetAttackRange(x, 25))
                    .OrderBy(x => unit.GetAngle(x.Position))
                    .FirstOrDefault(x => unit.GetAngle(x.Position) < 0.35f);
            }

            unit.IsAttacking = true;
            this.attackSleeper.Sleep(unit.Handle, unit.GetAttackPoint());

            this.AttackStart?.Invoke(unit);
        }

        private void AttackStopped(Unit9 unit)
        {
            if (!this.attackSleeper.IsSleeping(unit.Handle))
            {
                return;
            }

            if (!unit.IsControllable && !unit.IsTower && unit.IsHero)
            {
                unit.Target = null;
            }

            unit.IsAttacking = false;
            this.attackSleeper.Reset(unit.Handle);

            this.AttackEnd?.Invoke(unit);
        }

        private void CheckModifier(uint senderHandle, Modifier modifier, bool added)
        {
            var modifierName = modifier.Name;
            var unit = EntityManager9.GetUnitFast(senderHandle);

            if (unit == null)
            {
                return;
            }

            if (this.specialModifiers.TryGetValue(modifierName, out var action))
            {
                action(unit, modifier, added);
            }

            var range = this.rangeFactory.GetRange(modifierName);
            if (range != null)
            {
                unit.Range(range, added);
                return;
            }

            if (modifier.IsHidden)
            {
                return;
            }

            var disable = this.damageFactory.GetDisable(modifierName);
            if (disable != null)
            {
                var invulnerability = disable is IDisable disableAbility && (disableAbility.AppliesUnitState & UnitState.Invulnerable) != 0;
                unit.Disabler(modifier, added, invulnerability);
                return;
            }

            if (modifier.IsStunDebuff)
            {
                unit.Disabler(modifier, added, false);
                return;
            }

            var amplifier = this.damageFactory.GetAmplifier(modifierName);
            if (amplifier != null)
            {
                unit.Amplifier(amplifier, added);
                return;
            }

            var passive = this.damageFactory.GetPassive(modifierName);
            if (passive != null)
            {
                unit.Passive(passive, added);
                return;
            }

            var blocker = this.damageFactory.GetBlocker(modifierName);
            if (blocker != null)
            {
                unit.Blocker(blocker, added);
                return;
            }
        }

        private bool IsAttackAnimation(string animationName)
        {
            if (this.notAttackAnimation.Contains(animationName))
            {
                return false;
            }

            if (this.attackAnimation.Contains(animationName))
            {
                return true;
            }

            if (animationName.IndexOf("attack", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                this.attackAnimation.Add(animationName);
                return true;
            }

            this.notAttackAnimation.Add(animationName);
            return false;
        }

        private void OnAnimationChanged(Entity sender, AnimationChangedEventArgs e)
        {
            try
            {
                var unit = EntityManager9.GetUnit(sender.Handle);
                if (unit == null)
                {
                    return;
                }

                if (this.IsAttackAnimation(e.Name))
                {
                    this.AttackStarted(unit);
                }
                else
                {
                    this.AttackStopped(unit);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnGameEvent(GameEventEventArgs e)
        {
            try
            {
                var gameEvent = e.GameEvent;

                switch (gameEvent.Name)
                {
                    case "dota_player_kill":
                    case "dota_player_deny":
                        {
                            var id = gameEvent.GetInt32("victim_userid");
                            var handle = EntityManager.GetPlayerById(id)?.Hero?.Handle;
                            var unit = (Hero9)EntityManager9.GetUnitFast(handle);

                            if (unit == null || unit.Team == this.allyTeam)
                            {
                                break;
                            }

                            unit.BaseIsAlive = false;

                            if (unit.IsVisible)
                            {
                                var delay = (int)(((unit.BaseHero.RespawnTime - GameManager.RawGameTime) + 0.5f) * 1000);
                                if (delay <= 0)
                                {
                                    break;
                                }

                                UpdateManager.BeginInvoke(delay, () => RespawnUnit(unit));
                            }
                        }
                        break;
                    case "dota_buyback":
                        {
                            var id = gameEvent.GetInt32("player_id");
                            var handle = EntityManager.GetPlayerById(id)?.Hero?.Handle;
                            var unit = EntityManager9.GetUnitFast(handle);

                            if (unit == null || unit.Team == this.allyTeam)
                            {
                                break;
                            }

                            unit.BaseIsAlive = true;
                            unit.BaseHealth = unit.MaximumHealth;
                            unit.BaseMana = unit.MaximumMana;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                this.CheckModifier(modifier.Owner.Handle, modifier, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                this.CheckModifier(modifier.Owner.Handle, modifier, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}