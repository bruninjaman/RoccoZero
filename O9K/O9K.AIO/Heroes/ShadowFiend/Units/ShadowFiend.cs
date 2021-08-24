﻿namespace O9K.AIO.Heroes.ShadowFiend.Units
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using AIO.Abilities;
    using AIO.Abilities.Items;

    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Extensions;
    using Core.Helpers;
    using Core.Prediction.Data;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Game;
    using Divine.Order;
    using Divine.Particle;

    using Modes.Combo;

    using TargetManager;

    [UnitName(nameof(HeroId.npc_dota_hero_nevermore))]
    internal class ShadowFiend : ControllableUnit
    {
        private readonly List<NukeAbility> razes = new();

        private AbilityHelper abilityHelper;

        private ShieldAbility bkb;

        private BlinkAbility blink;

        private DisableAbility bloodthorn;

        private bool continueAltCombo;

        private EtherealBlade ethereal;

        private EulsScepterOfDivinity euls;

        private DisableAbility hex;

        private BuffAbility manta;

        private Nullifier nullifier;

        private DisableAbility orchid;

        private HurricanePike pike;

        private bool razeOrbwalk;

        private NukeAbility necromastery;

        private NukeAbility requiem;

        private DebuffAbility veil;

        public ShadowFiend(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
            : base(owner, abilitySleeper, orbwalkSleeper, menu)
        {
            this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
            {
                {
                    AbilityId.nevermore_shadowraze1, x =>
                    {
                        var raze = new NukeAbility(x);
                        this.razes.Add(raze);

                        return raze;
                    }
                },
                {
                    AbilityId.nevermore_shadowraze2, x =>
                    {
                        var raze = new NukeAbility(x);
                        this.razes.Add(raze);

                        return raze;
                    }
                },
                {
                    AbilityId.nevermore_shadowraze3, x =>
                    {
                        var raze = new NukeAbility(x);
                        this.razes.Add(raze);

                        return raze;
                    }
                },
                { AbilityId.nevermore_necromastery, x => this.necromastery = new NukeAbility(x) },
                { AbilityId.nevermore_requiem, x => this.requiem = new NukeAbility(x) },

                { AbilityId.item_veil_of_discord, x => this.veil = new DebuffAbility(x) },
                { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
                { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
                { AbilityId.item_nullifier, x => this.nullifier = new Nullifier(x) },
                { AbilityId.item_ethereal_blade, x => this.ethereal = new EtherealBlade(x) },
                { AbilityId.item_sheepstick, x => this.hex = new DisableAbility(x) },
                { AbilityId.item_manta, x => this.manta = new BuffAbility(x) },
                { AbilityId.item_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_swift_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_arcane_blink, x => this.blink = new BlinkDaggerShadowFiend(x) },
                { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkAbility(x) },
                { AbilityId.item_cyclone, x => this.euls = new EulsScepterOfDivinity(x) },
                { AbilityId.item_wind_waker, x => this.euls = new EulsScepterOfDivinity(x) },
                { AbilityId.item_hurricane_pike, x => this.pike = new HurricanePike(x) },
                { AbilityId.item_black_king_bar, x => this.bkb = new ShieldAbility(x) },
            };
        }

        public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
        {
            if (OrderManager.Orders.Count() != 0)
            {
                return false;
            }

            this.abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);
            var target = targetManager.Target;
            this.razeOrbwalk = false;

            if (this.blink?.Ability.Id == AbilityId.item_arcane_blink
                && comboModeMenu.GetAbilitySettingsMenu<BlinkDaggerShadowFiendMenu>(this.blink).DontUseEulInCombo)
            {
                if (this.AltUltCombo(targetManager, this.abilityHelper))
                {
                    return true;
                }
            }

            if (this.UltCombo(targetManager, this.abilityHelper))
            {
                return true;
            }

            if (this.abilityHelper.UseAbility(this.hex))
            {
                return true;
            }

            if (this.abilityHelper.UseAbility(this.bloodthorn))
            {
                return true;
            }

            if (this.abilityHelper.UseAbility(this.orchid))
            {
                return true;
            }

            if (this.abilityHelper.UseAbility(this.veil))
            {
                return true;
            }

            if (this.abilityHelper.UseAbility(this.nullifier))
            {
                return true;
            }

            if (this.abilityHelper.UseAbility(this.ethereal))
            {
                return true;
            }

            if (this.abilityHelper.UseAbility(this.pike, 800, 400))
            {
                return true;
            }

            if (this.abilityHelper.CanBeCasted(this.pike) && !this.MoveSleeper.IsSleeping)
            {
                if (this.pike.UseAbilityOnTarget(targetManager, this.ComboSleeper))
                {
                    return true;
                }
            }

            if (this.abilityHelper.UseAbility(this.manta, this.Owner.GetAttackRange()))
            {
                return true;
            }

            var orderedRazes = target.GetAngle(this.Owner.Position) > 1 || !target.IsMoving
                                   ? this.razes.OrderBy(x => x.Ability.Id)
                                   : this.razes.OrderByDescending(x => x.Ability.Id);

            foreach (var raze in orderedRazes)
            {
                var predictedPosition = target.GetPredictedPosition(1.5f);
                var distance = this.Owner.Distance(predictedPosition);

                if (!this.abilityHelper.CanBeCasted(raze))
                {
                    continue;
                }

                if (this.RazeCanWaitAttack(raze, target))
                {
                    continue;
                }

                if (!Divine.Helpers.MultiSleeper<string>.Sleeping("ShadowFiend.AIO.MoveToDirection") &&
                    raze.Ability.CastRange + raze.Ability.Radius > distance &&
                    raze.Ability.CastRange - raze.Ability.Radius < distance
                    && this.Owner.GetAngle(predictedPosition) > 0.4)
                {
                    this.Owner.BaseUnit.MoveToDirection(predictedPosition);

                    if (this.abilityHelper.ForceUseAbility(raze))
                    {
                        return true;
                    }

                    Divine.Helpers.MultiSleeper<string>.Sleep("ShadowFiend.AIO.MoveToDirection", 500);
                }

                if (this.abilityHelper.UseAbility(raze))
                {
                    return true;
                }
            }

            if (this.abilityHelper.UseAbility(this.necromastery))
            {
                return true;
            }

            this.razeOrbwalk = true;

            return false;
        }

        private bool AltUltCombo(TargetManager targetManager, AbilityHelper abilityHelper1)
        {
            if (!this.abilityHelper.CanBeCasted(this.requiem, false, false))
            {
                this.continueAltCombo = false;

                return false;
            }

            var target = targetManager.Target;
            var distance = this.Owner.Distance(target);
            var position = this.Owner.Position.Extend2D(target.Position, distance - 50);

            if (this.abilityHelper.CanBeCasted(this.blink))
            {
                var blinkRange = this.blink.Ability.CastRange;

                if (blinkRange >= distance)
                {
                    this.abilityHelper.UseAbility(this.bkb);

                    if (this.abilityHelper.UseAbility(this.blink, position))
                    {
                        this.continueAltCombo = true;

                        return true;
                    }
                }
            }

            if (this.continueAltCombo)
            {
                if (this.abilityHelper.UseAbility(this.bkb))
                {
                    return true;
                }

                if (this.abilityHelper.UseAbility(this.hex))
                {
                    this.ComboSleeper.ExtendSleep(0.1f);
                    this.OrbwalkSleeper.ExtendSleep(0.1f);

                    return true;
                }

                if (this.abilityHelper.UseAbility(this.veil))
                {
                    return true;
                }

                if (this.abilityHelper.UseAbility(this.ethereal))
                {
                    this.OrbwalkSleeper.Sleep(0.5f);

                    return true;
                }

                this.Owner.BaseUnit.MoveToDirection(target.Position);

                if (this.abilityHelper.UseAbility(this.requiem))
                {
                    this.continueAltCombo = false;

                    return true;
                }
            }

            this.continueAltCombo = false;

            return false;
        }

        public override bool Orbwalk(Unit9 target, bool attack, bool move, ComboModeMenu comboMenu = null)
        {
            if (this.razeOrbwalk && this.abilityHelper != null && target != null && this.Owner.Speed >= 305
                && target.GetImmobilityDuration() > 1)
            {
                var distance = this.Owner.Distance(target);

                foreach (var raze in this.razes.OrderBy(x => Math.Abs(x.Ability.CastRange - distance)))
                {
                    if (!this.abilityHelper.CanBeCasted(raze, false))
                    {
                        continue;
                    }

                    var position = target.Position.Extend2D(this.Owner.Position,
                        raze.Ability.CastRange - (raze.Ability.Radius - 100));

                    var distance2 = this.Owner.Distance(position);

                    if (target.GetImmobilityDuration() > 1)
                    {
                        if (distance2 < 250)
                        {
                            return this.Move(position);
                        }
                    }
                }
            }

            return base.Orbwalk(target, attack, move, comboMenu);
        }

        private bool RazeCanWaitAttack(UsableAbility raze, Unit9 target)
        {

            var damageFromRaze = raze.Ability.GetDamage(target);

            if (damageFromRaze > target.Health || this.Owner.GetAttackDamage(target) * 2 < damageFromRaze)
            {
                return false;
            }

            var input = raze.Ability.GetPredictionInput(target);

            if (this.MoveSleeper.IsSleeping)
            {
                input.Delay += this.MoveSleeper.RemainingSleepTime;
            }
            else if (!target.IsMoving || !this.Owner.CanAttack(target, -50))
            {
                return false;
            }
            else
            {
                input.Delay += this.Owner.GetAttackPoint(target) + this.Owner.GetTurnTime(target.Position);
            }

            var output = raze.Ability.GetPredictionOutput(input);

            if (output.HitChance < HitChance.Low)
            {
                return false;
            }

            return true;
        }

        private bool UltCombo(TargetManager targetManager, AbilityHelper abilityHelper)
        {
            if (!abilityHelper.CanBeCasted(this.requiem, false, false))
            {
                return false;
            }

            var target = targetManager.Target;
            var position = target.Position;
            var distance = this.Owner.Distance(position);
            var requiredTime = this.requiem.Ability.CastPoint + GameManager.Ping / 2000;
            const float AdditionalTime = 1f;

            if (target.IsInvulnerable)
            {
                var time = target.GetInvulnerabilityDuration();

                if (time <= 0)
                {
                    return true;
                }

                var eulsModifier = target.BaseModifiers.FirstOrDefault(x =>
                    x.Name == "modifier_eul_cyclone" || x.Name == "modifier_wind_waker");

                if (eulsModifier != null)
                {
                    var particle = ParticleManager.Particles.FirstOrDefault(x =>
                        x.Name == "particles/items_fx/cyclone.vpcf" && x.Owner == target.BaseEntity);

                    if (particle != null)
                    {
                        position = particle.GetControlPoint(0);
                        distance = this.Owner.Distance(position);
                    }
                }

                var remainingTime = time - requiredTime;

                if (remainingTime <= -AdditionalTime)
                {
                    return false;
                }

                if (distance < 150)
                {
                    if (abilityHelper.UseAbility(this.bkb))
                    {
                        return true;
                    }

                    if (remainingTime <= 0 && abilityHelper.ForceUseAbility(this.requiem))
                    {
                        return true;
                    }

                    if (!this.OrbwalkSleeper.IsSleeping && distance > 50)
                    {
                        this.OrbwalkSleeper.Sleep(0.1f);
                        this.Owner.BaseUnit.Move(position);
                    }

                    return true;
                }

                if (distance / this.Owner.Speed < remainingTime + AdditionalTime)
                {
                    if (abilityHelper.UseAbility(this.bkb))
                    {
                        return true;
                    }

                    this.OrbwalkSleeper.Sleep(0.1f);
                    this.ComboSleeper.Sleep(0.1f);

                    return this.Owner.BaseUnit.Move(position);
                }

                if (abilityHelper.CanBeCasted(this.blink))
                {
                    var blinkRange = this.blink.Ability.CastRange + this.Owner.Speed * remainingTime;

                    if (blinkRange > distance)
                    {
                        if (abilityHelper.UseAbility(this.blink, position))
                        {
                            this.OrbwalkSleeper.Sleep(0.1f);

                            return true;
                        }
                    }
                }
            }

            if (!abilityHelper.CanBeCasted(this.euls, false, false) || !this.euls.ShouldForceCast(targetManager) ||
                target.IsMagicImmune)
            {
                return false;
            }

            var eulsTime = this.euls.Ability.Duration - requiredTime;

            if (abilityHelper.CanBeCasted(this.blink))
            {
                var blinkRange = this.blink.Ability.CastRange + this.Owner.Speed * eulsTime;

                if (blinkRange > distance)
                {
                    if (abilityHelper.UseAbility(this.blink, position))
                    {
                        this.OrbwalkSleeper.Sleep(0.1f);
                        this.ComboSleeper.ExtendSleep(0.1f);

                        return true;
                    }
                }
            }

            if (distance / this.Owner.Speed < eulsTime)
            {
                if (abilityHelper.UseAbility(this.hex))
                {
                    this.ComboSleeper.ExtendSleep(0.1f);
                    this.OrbwalkSleeper.ExtendSleep(0.1f);

                    return true;
                }

                if (abilityHelper.UseAbility(this.veil))
                {
                    return true;
                }

                if (abilityHelper.UseAbility(this.ethereal))
                {
                    this.OrbwalkSleeper.Sleep(0.5f);

                    return true;
                }

                if (abilityHelper.ForceUseAbility(this.euls))
                {
                    return true;
                }
            }

            if (this.abilityHelper.CanBeCasted(this.requiem, false, false)
                && this.abilityHelper.CanBeCasted(this.euls, false, false)
                && !target.IsMagicImmune)
            {
                this.OrbwalkSleeper.Sleep(0.1f);

                return this.Owner.BaseUnit.Move(position);
            }

            return false;
        }
    }
}