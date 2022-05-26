﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BAIO.Heroes.Base;
using BAIO.Modes;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Extensions.SharpDX;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Renderer.Particle;
using log4net;
using PlaySharp.Toolkit.Logging;
using SharpDX;
using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;

namespace BAIO.Heroes.Modes.Combo
{
    internal class PugnaCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Pugna Pugna;

        public TaskHandler TurnTheOtherCheekHandler { get; private set; }

        public PugnaCombo(Pugna hero)
            : base(hero)
        {
            this.Pugna = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, 700);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Pugna.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            try
            {
                var linkens = Ensage.SDK.Extensions.UnitExtensions.IsLinkensProtected(this.CurrentTarget);

                await BreakLinken(token);
                var allTargets = this.Context.TargetSelector.Active.GetTargets().FirstOrDefault();

                var silenced = UnitExtensions.IsSilenced(this.Owner);

                var sliderValue = this.Pugna.UseBlinkPrediction.Item.GetValue<Slider>().Value;

                var myHpThreshold = this.Pugna.SelfHPDrain.Item.GetValue<Slider>().Value;

                var postDrainHp = this.Pugna.PostDrainHP.Item.GetValue<Slider>().Value;

                var allyPostDrain = this.Pugna.HealAllyTo.Item.GetValue<Slider>().Value;

                var healThreshold = this.Pugna.DrainHP.Item.GetValue<Slider>().Value;

                var wardTars = this.Pugna.WardTargets.Item.GetValue<Slider>().Value;

                if (!silenced)
                {
                    try
                    {
                        this.Pugna.HealTarget =
                            EntityManager<Hero>.Entities.FirstOrDefault(
                                x =>
                                    x.IsAlive && x.Team == this.Owner.Team && x != Owner && !x.IsIllusion
                                    && ((float) x.Health / (float) x.MaximumHealth) * 100 < healThreshold
                                    && !UnitExtensions.IsMagicImmune(x) &&
                                    this.Pugna.HealTargetHeroes.Value.IsEnabled(x.Name));

                        var myHealth = (float) Owner.Health / (float) Owner.MaximumHealth * 100;

                        if (this.Pugna.HealTarget != null)
                        {
                            this.Pugna.HealTarget = this.Pugna.HealTarget;
                        }

                        if (this.Pugna.HealTarget != null)
                        {
                            if (this.Pugna.HealTarget != null
                                && !UnitExtensions.IsChanneling(Owner) && myHealth >= myHpThreshold
                                && this.Pugna.HealTarget.Distance2D(this.Owner) <= this.Pugna.Drain.CastRange
                                && this.Pugna.HealTarget.HealthPercent() * 100 < healThreshold)
                            {
                                this.Pugna.Drain.UseAbility(this.Pugna.HealTarget);
                                this.Pugna.IsHealing = true;
                                await Task.Delay(
                                    (int) this.Pugna.Drain.GetCastDelay(this.Owner, this.Pugna.HealTarget, true),
                                    token);
                            }

                            //Stop Healing; There is no hidden modifier/any way to check if we are healing a target.
                            if ((UnitExtensions.IsChanneling(Owner) && myHealth <= postDrainHp) && this.Pugna.IsHealing)
                            {
                                Owner.Stop();
                                this.Pugna.IsHealing = false;
                            }

                            if (this.Pugna.HealTarget != null && this.Pugna.IsHealing &&
                                (this.Pugna.HealTarget.HealthPercent() >= ((float) allyPostDrain / 100)))
                            {
                                Owner.Stop();
                                this.Pugna.IsHealing = false;
                            }

                            if (this.Pugna.HealTarget == null && this.Pugna.IsHealing)
                            {
                                Owner.Stop();
                                this.Pugna.IsHealing = false;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                if (this.Pugna.IsHealing) return;

                if ((this.Pugna.BlinkDagger != null) &&
                    (this.Pugna.BlinkDagger.Item.IsValid) &&
                    this.CurrentTarget != null && Owner.Distance2D(this.CurrentTarget) <= 1200 + sliderValue &&
                    !(Owner.Distance2D(this.CurrentTarget) <= 400) &&
                    this.Pugna.BlinkDagger.Item.CanBeCasted(this.CurrentTarget)
                    && !UnitExtensions.IsChanneling(Owner))
                {
                    var l = (this.Owner.Distance2D(this.CurrentTarget) - sliderValue) / sliderValue;
                    var posA = this.Owner.Position;
                    var posB = this.CurrentTarget.Position;
                    var x = (posA.X + (l * posB.X)) / (1 + l);
                    var y = (posA.Y + (l * posB.Y)) / (1 + l);
                    var position = new Vector3((int) x, (int) y, posA.Z);

                    this.Pugna.BlinkDagger.UseAbility(position);
                    await Task.Delay((int) this.Pugna.BlinkDagger.GetCastDelay(position), token);
                }


                if (!silenced && this.CurrentTarget != null)
                {
                    var targets =
                        EntityManager<Hero>.Entities.Where(
                                x =>
                                    x.IsValid && x.Team != this.Owner.Team && !x.IsIllusion &&
                                    !UnitExtensions.IsMagicImmune(x) &&
                                    x.Distance2D(this.Owner) <= this.Pugna.Ward.GetAbilityData("radius"))
                            .ToList();

                    if (targets.Count >= wardTars && this.Pugna.Ward.CanBeCasted() &&
                        !UnitExtensions.IsChanneling(Owner))
                    {
                        this.Pugna.Ward.UseAbility(Owner.NetworkPosition);
                        await Task.Delay((int) this.Pugna.Ward.GetCastDelay(this.Owner, this.Owner, true), token);
                    }

                    try
                    {
                        // var thresholdTars = this.Config.WardTargets.Item.GetValue<Slider>();
                        var manaDecrepify = this.Pugna.Decrepify.GetManaCost(this.Pugna.Decrepify.Level - 1);
                        var manaBlast = this.Pugna.Blast.GetManaCost(this.Pugna.Blast.Level - 1);
                        // var manaDrain = Drain.GetManaCost(Drain.Level - 1);

                        if (this.Pugna.Decrepify.CanBeCasted() && this.CurrentTarget != null &&
                            this.Pugna.Decrepify.CanHit(this.CurrentTarget)
                            && this.Owner.Mana >= manaBlast + manaDecrepify
                            && !UnitExtensions.IsChanneling(Owner)
                            && this.CurrentTarget.IsAlive)
                        {
                            this.Pugna.Decrepify.UseAbility(this.CurrentTarget);
                            await Task.Delay(
                                (int) this.Pugna.Decrepify.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                        }

                        if (this.Pugna.Blast.CanBeCasted()
                            && (!this.Pugna.Decrepify.CanBeCasted() || manaBlast > Owner.Mana - manaDecrepify)
                            && !UnitExtensions.IsChanneling(Owner)
                            && this.CurrentTarget != null && this.CurrentTarget.IsAlive)
                        {
                            var delay = this.Pugna.Blast.GetAbilityData("delay") + this.Pugna.Blast.GetCastPoint();
                            var blastTargets =
                                EntityManager<Hero>.Entities.OrderBy(x => x == allTargets).Where(
                                    x =>
                                        x.IsValid && x.IsVisible && x.Team != Owner.Team && !x.IsIllusion &&
                                        !UnitExtensions.IsMagicImmune(x)).ToList();
                            var blastCastRange = this.Pugna.Blast.CastRange;

                            if (blastTargets == null) return;
                            var input =
                                new PredictionInput(
                                    Owner,
                                    this.CurrentTarget,
                                    delay,
                                    float.MaxValue,
                                    blastCastRange,
                                    400,
                                    PredictionSkillshotType.SkillshotCircle,
                                    true,
                                    blastTargets)
                                {
                                    CollisionTypes = CollisionTypes.None
                                };

                            var output = this.Context.Prediction.GetPrediction(input);

                            if (output.HitChance >= HitChance.Medium)
                            {
                                this.Pugna.Blast.UseAbility(output.CastPosition);
                                await Task.Delay(
                                    (int) this.Pugna.Blast.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (this.Pugna.BloodThorn != null &&
                    this.Pugna.BloodThorn.Item.IsValid &&
                    this.CurrentTarget != null && !UnitExtensions.IsChanneling(Owner) &&
                    this.Pugna.BloodThorn.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.BloodThorn.UseAbility(this.CurrentTarget);
                    await Task.Delay(this.Pugna.BloodThorn.GetCastDelay(this.CurrentTarget), token);
                }

                if ((this.Pugna.Sheepstick != null) &&
                    (this.Pugna.Sheepstick.Item.IsValid) &&
                    this.CurrentTarget != null && !UnitExtensions.IsChanneling(Owner) &&
                    this.Pugna.Sheepstick.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.Sheepstick.UseAbility(this.CurrentTarget);
                    await Await.Delay(this.Pugna.Sheepstick.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.Dagon != null &&
                    this.Pugna.Dagon.Item.IsValid &&
                    this.CurrentTarget != null && !UnitExtensions.IsChanneling(Owner) &&
                    this.Pugna.Dagon.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.Dagon.UseAbility(this.CurrentTarget);
                    await Await.Delay(this.Pugna.Dagon.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.Orchid != null &&
                    this.Pugna.Orchid.Item.IsValid &&
                    this.CurrentTarget != null && !UnitExtensions.IsChanneling(Owner) &&
                    this.Pugna.Orchid.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.Orchid.UseAbility(this.CurrentTarget);
                    await Await.Delay(this.Pugna.Orchid.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.RodOfAtos != null &&
                    this.Pugna.RodOfAtos.Item.IsValid &&
                    this.CurrentTarget != null && !UnitExtensions.IsChanneling(Owner) &&
                    this.Pugna.RodOfAtos.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.RodOfAtos.UseAbility(this.CurrentTarget);
                    await Await.Delay(this.Pugna.RodOfAtos.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.VeilOfDiscord != null &&
                    this.Pugna.VeilOfDiscord.Item.IsValid &&
                    this.CurrentTarget != null && !UnitExtensions.IsChanneling(Owner) &&
                    this.Pugna.VeilOfDiscord.Item.CanBeCasted())
                {
                    this.Pugna.VeilOfDiscord.UseAbility(this.CurrentTarget.Position);
                    await Await.Delay(this.Pugna.VeilOfDiscord.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.ShivasGuard != null &&
                    this.Pugna.ShivasGuard.Item.IsValid &&
                    this.CurrentTarget != null && !UnitExtensions.IsChanneling(Owner) &&
                    this.Pugna.ShivasGuard.Item.CanBeCasted() &&
                    Owner.Distance2D(this.CurrentTarget) <= 900)
                {
                    this.Pugna.ShivasGuard.UseAbility();
                    await Await.Delay((int) Game.Ping, token);
                }

                if (!silenced && this.Pugna.Drain.CanBeCasted() &&
                    (!this.Pugna.Blast.CanBeCasted() || this.Owner.Distance2D(this.CurrentTarget) > this.Pugna.Blast.CastRange)
                    && (!this.Pugna.Decrepify.CanBeCasted() || this.Owner.Distance2D(this.CurrentTarget) > this.Pugna.Decrepify.CastRange)
                    && !UnitExtensions.IsChanneling(Owner)
                    && this.CurrentTarget != null && this.CurrentTarget.IsAlive)
                {
                    this.Pugna.Drain.UseAbility(this.CurrentTarget);
                    await Task.Delay((int) this.Pugna.Drain.GetCastDelay(this.Owner, this.CurrentTarget, true) + 50,
                        token);
                }
                else if (!silenced && this.Pugna.Drain.CanBeCasted()
                                   && (this.Owner.Distance2D(this.CurrentTarget) > this.Pugna.Blast.CastRange
                                       || this.Owner.Distance2D(this.CurrentTarget) > this.Pugna.Decrepify.CastRange)
                                   && !UnitExtensions.IsChanneling(Owner)
                                   && this.CurrentTarget != null && this.CurrentTarget.IsAlive)
                {
                    this.Pugna.Drain.UseAbility(this.CurrentTarget);
                    await Task.Delay((int) this.Pugna.Drain.GetCastDelay(this.Owner, this.CurrentTarget, true) + 50,
                        token);
                }

                if (this.CurrentTarget != null && !Owner.IsValidOrbwalkingTarget(this.CurrentTarget) &&
                    !UnitExtensions.IsChanneling(this.Owner))
                {
                    Orbwalker.Move(Game.MousePosition);
                    await Await.Delay(50, token);
                }
                else
                {
                    Orbwalker.OrbwalkTo(this.CurrentTarget);
                }

                await Await.Delay(50, token);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                Log.Debug($"{e}");
            }

            this.OrbwalkToTarget();
        }

        protected async Task BreakLinken(CancellationToken token)
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<string, uint>> breakerChanger = new List<KeyValuePair<string, uint>>();

                    if (Ensage.SDK.Extensions.UnitExtensions.IsLinkensProtected(this.CurrentTarget))
                    {
                        breakerChanger = this.Pugna.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Pugna.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Pugna.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Pugna.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Pugna.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Pugna.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Pugna.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(
                                nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget),
                                token);
                            return;
                        }

                        var atos = this.Pugna.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(
                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Pugna.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Pugna.DiffusalBlade;
                        if (diff != null
                            && diff.ToString() == order.Key
                            && diff.CanBeCasted && diff.CanHit(this.CurrentTarget))
                        {
                            diff.UseAbility(this.CurrentTarget);
                            await Task.Delay(diff.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }
                catch (Exception e)
                {
                    Log.Error("Linken break error: " + e);
                }
            }
        }
    }
}