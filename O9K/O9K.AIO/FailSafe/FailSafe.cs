namespace O9K.AIO.FailSafe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Base.Components;
    using Core.Entities.Abilities.Base.Types;
    using Core.Extensions;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu.EventArgs;
    using Core.Prediction.Data;

    using Divine;

    using Heroes.Base;

    using KillStealer;

    using Modes.Base;

    using O9K.Core.Geometry;

    using SharpDX;

    internal class FailSafe : BaseMode
    {
        private readonly Dictionary<uint, Vector3> abilityPositions = new Dictionary<uint, Vector3>();

        private readonly Dictionary<ActiveAbility, float> abilityTimings = new Dictionary<ActiveAbility, float>();

        private readonly UpdateHandler failSafeHandler;

        private readonly HashSet<AbilityId> ignoredAbilities = new HashSet<AbilityId>
        {
            AbilityId.arc_warden_magnetic_field,
            AbilityId.storm_spirit_ball_lightning,
            AbilityId.leshrac_diabolic_edict,
            AbilityId.shredder_timber_chain,
            AbilityId.magnataur_skewer,
            AbilityId.disruptor_kinetic_field,
            AbilityId.nevermore_requiem,
            AbilityId.disruptor_static_storm,
            AbilityId.crystal_maiden_freezing_field,
            AbilityId.skywrath_mage_mystic_flare,
            AbilityId.phoenix_icarus_dive, // todo fail safe only with combo key
            AbilityId.kunkka_torrent, // todo x mark check
            AbilityId.kunkka_ghostship, // todo x mark check
            AbilityId.elder_titan_echo_stomp_spirit,
            AbilityId.elder_titan_echo_stomp,
            AbilityId.bloodseeker_blood_bath,
            AbilityId.phantom_lancer_doppelwalk,
            AbilityId.mars_gods_rebuke, //todo fix
            AbilityId.beastmaster_wild_axes, //todo fix
        };

        private readonly KillSteal killSteal;

        private readonly FailSafeMenu menu;

        public FailSafe(BaseHero baseHero)
            : base(baseHero)
        {
            this.killSteal = baseHero.KillSteal;
            this.AbilitySleeper = baseHero.AbilitySleeper;
            this.OrbwalkSleeper = baseHero.OrbwalkSleeper;
            this.menu = new FailSafeMenu(baseHero.Menu.GeneralSettingsMenu);
            this.failSafeHandler = UpdateManager.CreateIngameUpdate(0, false, this.OnUpdate);
        }

        public MultiSleeper AbilitySleeper { get; }

        public bool Enabled
        {
            get
            {
                return this.menu.FailSafeEnabled;
            }
        }

        public MultiSleeper OrbwalkSleeper { get; }

        public Sleeper Sleeper { get; } = new Sleeper();

        public void Disable()
        {
            this.menu.FailSafeEnabled.ValueChange -= this.FailSafeEnabledOnValueChanged;
            this.FailSafeEnabledOnValueChanged(null, new SwitcherEventArgs(false, false));
        }

        public override void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.failSafeHandler);
            Entity.NetworkPropertyChanged += this.OnNetworkPropertyChanged;
            OrderManager.OrderAdding += this.OnOrderAdding;
            this.menu.FailSafeEnabled.ValueChange -= this.FailSafeEnabledOnValueChanged;
        }

        public void Enable()
        {
            this.menu.FailSafeEnabled.ValueChange += this.FailSafeEnabledOnValueChanged;
        }

        private void FailSafeEnabledOnValueChanged(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                Entity.NetworkPropertyChanged += this.OnNetworkPropertyChanged;
                OrderManager.OrderAdding += this.OnOrderAdding;
            }
            else
            {
                this.failSafeHandler.IsEnabled = false;
                Entity.NetworkPropertyChanged -= this.OnNetworkPropertyChanged;
                OrderManager.OrderAdding -= this.OnOrderAdding;
            }
        }

        private bool IsIgnored(Ability9 ability)
        {
            if (this.ignoredAbilities.Contains(ability.Id))
            {
                return true;
            }

            return ability is IBlink;
        }

        private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "m_bInAbilityPhase")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    var newValue = e.NewValue.GetBoolean();
                    if (newValue == e.OldValue.GetBoolean())
                    {
                        return;
                    }

                    var ability = EntityManager9.GetAbility(sender.Handle) as ActiveAbility;
                    if (ability?.IsControllable != true)
                    {
                        return;
                    }

                    if (this.IsIgnored(ability))
                    {
                        return;
                    }

                    if (!(ability is AreaOfEffectAbility) && !(ability is PredictionAbility))
                    {
                        return;
                    }

                    if (newValue)
                    {
                        if (ability is AreaOfEffectAbility)
                        {
                            if (ability.CastRange > 0)
                            {
                                UpdateManager.BeginInvoke(10, () => this.abilityPositions[ability.Handle] = ability.Owner.InFront(ability.CastRange));
                            }
                            else
                            {
                                UpdateManager.BeginInvoke(10, () => this.abilityPositions[ability.Handle] = ability.Owner.Position);
                            }
                        }

                        this.abilityTimings[ability] = GameManager.RawGameTime + ability.CastPoint;
                        this.failSafeHandler.IsEnabled = true;
                    }
                    else
                    {
                        this.abilityTimings.Remove(ability);
                        this.abilityPositions.Remove(ability.Handle);
                        if (this.abilityTimings.Count <= 0)
                        {
                            this.failSafeHandler.IsEnabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (!e.Process || !e.IsCustom)
                {
                    return;
                }

                var order = e.Order;
                if (order.Type != OrderType.CastPosition)
                {
                    return;
                }

                var ability = EntityManager9.GetAbility(order.Ability.Handle);
                if (ability == null)
                {
                    return;
                }

                if (this.IsIgnored(ability))
                {
                    return;
                }

                this.abilityPositions[ability.Handle] = order.Position;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnUpdate()
        {
            try
            {
                if (this.Sleeper.IsSleeping)
                {
                    return;
                }

                var target = this.TargetManager.TargetLocked ? this.TargetManager.Target : this.killSteal.Target;
                if (target?.IsValid != true || !target.IsVisible)
                {
                    return;
                }

                foreach (var abilityTiming in this.abilityTimings.ToList())
                {
                    var ability = abilityTiming.Key;
                    if (!ability.IsValid || !ability.BaseAbility.IsInAbilityPhase)
                    {
                        continue;
                    }

                    var owner = ability.Owner;
                    var phaseEnd = abilityTiming.Value;

                    var input = ability.GetPredictionInput(target);
                    input.Delay = Math.Max(phaseEnd - GameManager.RawGameTime, 0) + ability.ActivationDelay;
                    var output = ability.GetPredictionOutput(input);

                    if (!(ability is IHasRadius))
                    {
                        continue;
                    }

                    if (!this.abilityPositions.TryGetValue(ability.Handle, out var castPosition))
                    {
                        continue;
                    }

                    Polygon polygon = null;

                    switch (ability)
                    {
                        case AreaOfEffectAbility aoe:
                        {
                            polygon = new Polygon.Circle(castPosition, aoe.Radius + 50);
                            break;
                        }
                        case CircleAbility circle:
                        {
                            polygon = new Polygon.Circle(castPosition, circle.Radius + 50);
                            break;
                        }
                        case ConeAbility cone:
                        {
                            polygon = new Polygon.Trapezoid(
                                owner.Position.Extend2D(castPosition, -cone.Radius / 2),
                                owner.Position.Extend2D(castPosition, cone.Range),
                                cone.Radius + 50,
                                cone.EndRadius + 100);
                            break;
                        }
                        case LineAbility line:
                        {
                            polygon = new Polygon.Rectangle(
                                owner.Position,
                                owner.Position.Extend2D(castPosition, line.Range),
                                line.Radius + 50);
                            break;
                        }
                    }

                    if (polygon == null)
                    {
                        continue;
                    }

                    if (!target.IsAlive || output.HitChance == HitChance.Impossible || !polygon.IsInside(output.TargetPosition))
                    {
                        this.Sleeper.Sleep(0.15f);
                        this.abilityTimings.Remove(ability);
                        this.abilityPositions.Remove(ability.Handle);
                        this.OrbwalkSleeper.Reset(ability.Owner.Handle);
                        this.AbilitySleeper.Reset(ability.Handle);
                        // this.TargetSleeper.Reset(target.Handle);
                        this.killSteal.KillStealSleeper.Reset();
                        target.RefreshUnitState();
                        ability.Owner.BaseUnit.Stop();
                        var unit = this.BaseHero.UnitManager.AllControllableUnits.FirstOrDefault(x => x.Handle == ability.Owner.Handle);
                        unit?.ComboSleeper.Reset();
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