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
using Ensage.SDK.Abilities.npc_dota_hero_grimstroke;
using log4net;
using PlaySharp.Toolkit.Logging;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;
using SharpDX;
using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;
using UnitExtensions = Ensage.Common.Extensions.UnitExtensions;
using Ensage.SDK.Geometry;
using Ensage.SDK.Renderer.Particle;

namespace BAIO.Heroes.Modes.Combo
{
    internal class GrimstrokeCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Grimstroke Grimstroke;

        public TaskHandler IllusionHandler { get; private set; }

        private Vector3 sofPredictionPosition;

        private float sofStartCastTime;

        private readonly IUpdateHandler sofUpdateHandler;

        private IParticleManager particle { get; set; }


        public GrimstrokeCombo(Grimstroke hero)
            : base(hero)
        {
            Grimstroke = hero;
            Entity.OnBoolPropertyChange += this.OnSofCast;
            particle = this.Context.Particle;
            this.IllusionHandler = UpdateManager.Run(OnUpdate);
            this.sofUpdateHandler = UpdateManager.Subscribe(this.SofHitCheck, 0, false);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await ShouldExecute(token))
            {
                return;
            }

            var SoF = Grimstroke.StrokeOfFate;
            var Dp = Grimstroke.DarkPortrait;
            if (SoF != null)
            {
                MaxTargetRange = SofCastRange;
            }

            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible)
            {
                Grimstroke.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                OrbwalkToTarget();
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            var phantom = Grimstroke.PhantomsEmbrace;
            var ink = Grimstroke.InkSwell;
            var ult = Grimstroke.Soulbind;

            var targetDistance = Owner.Distance2D(this.CurrentTarget);
            var Delay = 1.33f; // https://dota2.gamepedia.com/Grimstroke

            var SoFSpeed = Grimstroke.StrokeOfFate.Speed;

            var castUlti = this.CurrentTarget.GetAlliesInRange<Hero>(ult.Ability.GetAbilitySpecialData("chain_latch_radius")).Count() >= 1;
            if (ult != null && AbilityExtensions.CanBeCasted(ult, this.CurrentTarget) && castUlti)
            {
                var ultModifier = "modifier_grimstroke_soul_chain";
                var canCastMyShit = this.CurrentTarget.HasModifier(ultModifier);
                if (this.Owner.Distance2D(this.CurrentTarget) < ult.CastRange)
                {
                    this.Grimstroke.Soulbind.UseAbility(this.CurrentTarget);
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(ult, this.Owner, this.CurrentTarget), token);
                }

                if (Dp != null && AbilityExtensions.CanBeCasted(Dp, this.CurrentTarget) && !linkens && canCastMyShit &&
                    this.Grimstroke.PortraitToggler.Value.IsEnabled(CurrentTarget.Name) &&
                    this.Owner.Distance2D(this.CurrentTarget) <= Dp.CastRange)
                {
                    this.Grimstroke.DarkPortrait.UseAbility(this.CurrentTarget);
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(Dp, this.Owner, this.CurrentTarget), token);
                }

                if (phantom != null && AbilityExtensions.CanBeCasted(phantom, this.CurrentTarget) && !linkens && canCastMyShit &&
                    this.Owner.Distance2D(this.CurrentTarget) <= phantom.CastRange)
                {
                    this.Grimstroke.PhantomsEmbrace.UseAbility(this.CurrentTarget);
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(phantom, this.Owner, this.CurrentTarget), token);
                }

                var heroWithMostBesideHim = EntityManager<Hero>.Entities.Where(a =>
                    a != null && a.Team == this.Owner.Team && a.GetEnemiesInRange<Hero>(InkSwellRadius).Count() >= 1).FirstOrDefault();

                if (ink != null && AbilityExtensions.CanBeCasted(ink) && heroWithMostBesideHim != null &&
                    this.Owner.Distance2D(heroWithMostBesideHim) <= ink.CastRange)
                {
                    this.Grimstroke.InkSwell.UseAbility(heroWithMostBesideHim);
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(ink, this.Owner, heroWithMostBesideHim), token);
                }

                var bloodthorn = this.Grimstroke.BloodThorn;
                if (bloodthorn != null && bloodthorn.CanBeCasted && canCastMyShit && bloodthorn.CanHit(CurrentTarget) && !linkens)
                {
                    bloodthorn.UseAbility(this.CurrentTarget);
                    await Task.Delay(bloodthorn.GetCastDelay(), token);
                }

                var orchid = this.Grimstroke.Orchid;
                if (orchid != null && orchid.CanBeCasted && canCastMyShit && orchid.CanHit(CurrentTarget) && !linkens)
                {
                    orchid.UseAbility(this.CurrentTarget);
                    await Task.Delay(orchid.GetCastDelay(), token);
                }

                var ethereal = this.Grimstroke.EtherealBlade;
                if (ethereal != null && ethereal.CanBeCasted && canCastMyShit && ethereal.CanHit(CurrentTarget) && !linkens)
                {
                    ethereal.UseAbility(this.CurrentTarget);
                    await Task.Delay(ethereal.GetCastDelay(), token);
                }

                var dagon = Grimstroke.Dagon;
                if (dagon != null && !linkens && !this.CurrentTarget.IsIllusion && dagon.CanBeCasted && canCastMyShit &&
                    dagon.CanHit(this.CurrentTarget))
                {
                    dagon.UseAbility(this.CurrentTarget);
                    await Task.Delay(dagon.GetCastDelay(), token);
                }

                var sheepStick = this.Grimstroke.Sheepstick;
                if (sheepStick != null && sheepStick.CanBeCasted && sheepStick.CanHit(CurrentTarget) && !linkens && canCastMyShit)
                {
                    sheepStick.UseAbility(this.CurrentTarget);
                    await Task.Delay(sheepStick.GetCastDelay(), token);
                }

                var mjollnir = Grimstroke.Mjollnir;
                if (mjollnir != null && mjollnir.CanBeCasted)
                {
                    mjollnir.UseAbility(Owner);
                    await Task.Delay(mjollnir.GetCastDelay(), token);
                }

                var diffusal = Grimstroke.DiffusalBlade;
                if (diffusal != null && !linkens && !this.CurrentTarget.IsIllusion && diffusal.CanBeCasted &&
                    diffusal.CanHit(this.CurrentTarget) && canCastMyShit)
                {
                    diffusal.UseAbility(CurrentTarget);
                    await Task.Delay(diffusal.GetCastDelay(), token);
                }

                var nullfier = Grimstroke.Nullifier;
                if (nullfier != null && !linkens && !this.CurrentTarget.IsIllusion && nullfier.CanBeCasted &&
                    nullfier.CanHit(this.CurrentTarget) && canCastMyShit)
                {
                    nullfier.UseAbility(this.CurrentTarget);
                    await Task.Delay(nullfier.GetCastDelay(this.CurrentTarget), token);
                }
            }
            else
            {
                if (phantom != null && AbilityExtensions.CanBeCasted(phantom, this.CurrentTarget) && !linkens &&
                    this.Owner.Distance2D(this.CurrentTarget) <= phantom.CastRange)
                {
                    this.Grimstroke.PhantomsEmbrace.UseAbility(this.CurrentTarget);
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(phantom, this.Owner, this.CurrentTarget), token);
                }

                if (Dp != null && AbilityExtensions.CanBeCasted(Dp, this.CurrentTarget) && !linkens &&
                    this.Owner.Distance2D(this.CurrentTarget) <= Dp.CastRange &&
                    this.Grimstroke.PortraitToggler.Value.IsEnabled(CurrentTarget.Name))
                {
                    this.Grimstroke.DarkPortrait.UseAbility(this.CurrentTarget);
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(Dp, this.Owner, this.CurrentTarget), token);
                }

                var heroWithMostBesideHim = EntityManager<Hero>.Entities.Where(a =>
                    a != null && a.Team == this.Owner.Team && a.GetEnemiesInRange<Hero>(InkSwellRadius).Count() >= 1).FirstOrDefault();
                if (ink != null && AbilityExtensions.CanBeCasted(ink) && heroWithMostBesideHim != null &&
                    this.Owner.Distance2D(heroWithMostBesideHim) <= ink.CastRange)
                {
                    this.Grimstroke.InkSwell.UseAbility(heroWithMostBesideHim);
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(ink, this.Owner, heroWithMostBesideHim), token);
                }

                var bloodthorn = this.Grimstroke.BloodThorn;
                if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
                    this.Grimstroke.BloodthornOrchidHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    bloodthorn.UseAbility(this.CurrentTarget);
                    await Task.Delay(bloodthorn.GetCastDelay(), token);
                }

                var orchid = this.Grimstroke.Orchid;
                if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
                    this.Grimstroke.BloodthornOrchidHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    orchid.UseAbility(this.CurrentTarget);
                    await Task.Delay(orchid.GetCastDelay(), token);
                }

                var ethereal = this.Grimstroke.EtherealBlade;
                if (ethereal != null && ethereal.CanBeCasted && ethereal.CanHit(CurrentTarget) && !linkens)
                {
                    ethereal.UseAbility(this.CurrentTarget);
                    await Task.Delay(ethereal.GetCastDelay(), token);
                }

                var dagon = Grimstroke.Dagon;
                if (dagon != null && !linkens && !this.CurrentTarget.IsIllusion && dagon.CanBeCasted &&
                    dagon.CanHit(this.CurrentTarget))
                {
                    dagon.UseAbility(this.CurrentTarget);
                    await Task.Delay(dagon.GetCastDelay(), token);
                }

                var sheepStick = this.Grimstroke.Sheepstick;
                if (sheepStick != null && sheepStick.CanBeCasted && sheepStick.CanHit(CurrentTarget) && !linkens)
                {
                    sheepStick.UseAbility(this.CurrentTarget);
                    await Task.Delay(sheepStick.GetCastDelay(), token);
                }

                var mjollnir = Grimstroke.Mjollnir;
                if (mjollnir != null && mjollnir.CanBeCasted)
                {
                    mjollnir.UseAbility(Owner);
                    await Task.Delay(mjollnir.GetCastDelay(), token);
                }

                var diffusal = Grimstroke.DiffusalBlade;
                if (diffusal != null && !linkens && !this.CurrentTarget.IsIllusion && diffusal.CanBeCasted &&
                    diffusal.CanHit(this.CurrentTarget))
                {
                    diffusal.UseAbility(CurrentTarget);
                    await Task.Delay(diffusal.GetCastDelay(), token);
                }

                var nullfier = Grimstroke.Nullifier;
                if (nullfier != null && !linkens && !this.CurrentTarget.IsIllusion && nullfier.CanBeCasted &&
                    nullfier.CanHit(this.CurrentTarget) && this.Grimstroke.NullifierHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    nullfier.UseAbility(this.CurrentTarget);
                    await Task.Delay(nullfier.GetCastDelay(this.CurrentTarget), token);
                }
            }

            if (SoF != null && AbilityExtensions.CanBeCasted(SoF) &&
                this.Owner.Distance2D(this.CurrentTarget) <= SofCastRange)
            {
                var sofInput = Grimstroke.StrokeOfFate.GetPredictionInput(this.CurrentTarget);
                var sofOutput = Grimstroke.StrokeOfFate.GetPredictionOutput(sofInput);

                if (this.CanSofHit(sofOutput))
                {
                    this.sofPredictionPosition = sofOutput.CastPosition;
                    this.Grimstroke.StrokeOfFate.UseAbility(sofPredictionPosition);
                    await Task.Delay((int)Delay, token);
                }
            }

            this.OrbwalkToTarget();
        }

        private void OnSofCast(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (args.NewValue == args.OldValue || sender != this.Grimstroke.StrokeOfFate ||
                args.PropertyName != "m_bInAbilityPhase")
            {
                return;
            }

            if (args.NewValue)
            {
                this.sofStartCastTime = Game.RawGameTime;
                this.sofUpdateHandler.IsEnabled = true;
            }
            else
            {
                this.sofUpdateHandler.IsEnabled = false;
            }
        }

        private void SofHitCheck()
        {
            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible)
            {
                return;
            }

            var sof = this.Grimstroke.StrokeOfFate;
            var input = sof.GetPredictionInput(this.CurrentTarget);
            input.Delay = Math.Max((this.sofStartCastTime - Game.RawGameTime) + sof.CastPoint, 0);
            var output = sof.GetPredictionOutput(input);

            if (this.sofPredictionPosition.Distance2D(output.CastPosition) > sof.Radius * 1.9f || !this.CanSofHit(output))
            {
                this.Owner.Stop();
                this.Cancel();
                this.sofUpdateHandler.IsEnabled = false;
            }
        }



        private bool CanSofHit(PredictionOutput output)
        {
            switch (output.HitChance)
            {
                case HitChance.OutOfRange:
                    return false;
                case HitChance.Impossible:
                    return false;
                case HitChance.Collision:
                    return false;
                case HitChance.Immobile:
                    break;
                case HitChance.Dashing:
                    break;
                case HitChance.VeryHigh:
                    break;
                case HitChance.High:
                    break;
                case HitChance.Medium:
                    break;
                case HitChance.Low:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return output.HitChance >= HitChance.Low;
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Grimstroke.Config.General.ComboKey.Value.Active)
            {
                await Task.Delay(250, token);
                return;
            }

            if (this.Grimstroke.DarkPortrait == null && this.Owner.HasAghanimsScepter())
            {
                this.Grimstroke.DarkPortrait = this.Context.AbilityFactory.GetAbility<grimstroke_scepter>();
            }

            var illusions =
                EntityManager<Unit>.Entities.Where(
                        x => x.IsValid && x.IsAlive && x.IsIllusion && x.Team == this.Owner.Team && x.IsControllable)
                    .ToList();

            if (illusions.Any())
            {
                foreach (var illusion in illusions)
                {
                    illusion.Attack(this.CurrentTarget);
                    await Task.Delay(100, token);
                }
            }
            await Task.Delay(100, token);
        }

        private float InkSwellRadius
        {
            get
            {
                var radius = this.Grimstroke.InkSwell.Ability.GetAbilitySpecialData("radius");
                var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_grimstroke_1);

                if (talent?.Level > 0)
                {
                    radius += talent.GetAbilitySpecialData("value");
                }

                return radius;
            }
        }

        private float SofCastRange
        {
            get
            {
                var SoFCastRange = (float)Grimstroke.StrokeOfFate.CastRange;
                var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_grimstroke_3);

                if (talent?.Level > 0)
                {
                    SoFCastRange += 600f;
                }

                return SoFCastRange;
            }
        }


        protected async Task BreakLinken(CancellationToken token)
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<string, uint>> breakerChanger = new List<KeyValuePair<string, uint>>();

                    if (this.CurrentTarget.IsLinkensProtected())
                    {
                        breakerChanger = this.Grimstroke.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Grimstroke.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Grimstroke.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Grimstroke.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Grimstroke.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Grimstroke.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Grimstroke.Nullifier;
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

                        var atos = this.Grimstroke.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(
                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Grimstroke.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Grimstroke.DiffusalBlade;
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