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
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;
using log4net;
using PlaySharp.Toolkit.Logging;
using SharpDX;
using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

namespace BAIO.Heroes.Modes.Combo
{
    internal class SpectreCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Spectre Spectre;

        public TaskHandler IllusionHandler { get; private set; }

        public TaskHandler RealityHandler { get; private set; }

        private List<Unit> HauntIllusions { get; set; }

        public SpectreCombo(Spectre hero)
            : base(hero)
        {
            this.Spectre = hero;
            this.IllusionHandler = UpdateManager.Run(OnUpdate);
            this.RealityHandler = UpdateManager.Run(RealityUsage);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            var hauntIllusions = EntityManager<Unit>.Entities.Where(x => x.Name == this.Owner.Name && x.IsAlive && x.IsIllusion && !x.IsInvulnerable()
            && x.IsAlly(this.Owner) && x.HasModifier("modifier_spectre_haunt"));

            MaxTargetRange = AbilityExtensions.CanBeCasted(this.Spectre.Haunt) ? float.MaxValue : 2000;

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Spectre.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            try
            {
                var linkens = this.CurrentTarget.IsLinkensProtected();
                await BreakLinken(token);

                var spectraldagger = this.Spectre.SpectralDagger;
                if (AbilityExtensions.CanBeCasted(spectraldagger) && this.Owner.Distance2D(this.CurrentTarget) <= spectraldagger.CastRange)
                {
                    spectraldagger.UseAbility(this.CurrentTarget);
                    await Task.Delay(
                        (int) AbilityExtensions.GetCastDelay(spectraldagger, this.Owner, this.CurrentTarget), token);
                }

                var shadowstep = this.Spectre.ShadowStep;
                if (AbilityExtensions.CanBeCasted(shadowstep) &&
                    this.Owner.Distance2D(this.CurrentTarget) >= this.Spectre.MinimumUltiDistance.Value)
                {
                    shadowstep.UseAbility(this.CurrentTarget);
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(shadowstep, this.Owner, this.CurrentTarget), token);
                    await Task.Delay(250, token);
                }

                var haunt = this.Spectre.Haunt;
                if (AbilityExtensions.CanBeCasted(haunt) && !AbilityExtensions.CanBeCasted(shadowstep) &&
                    !HauntIllusions.Any(x => x.Distance2D(this.CurrentTarget) <= 300) &&
                    this.Owner.Distance2D(this.CurrentTarget) >= this.Spectre.MinimumUltiDistance.Value)
                {
                    haunt.UseAbility();
                    await Task.Delay((int)AbilityExtensions.GetCastDelay(haunt, this.Owner, this.CurrentTarget), token);
                }

                var abyssal = this.Spectre.AbyssalBlade;
                if (abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
                    this.Spectre.AbyssalBladeHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    abyssal.UseAbility(CurrentTarget);
                    await Task.Delay(abyssal.GetCastDelay(), token);
                }

                var diff = this.Spectre.DiffusalBlade;
                if (diff != null && diff.CanBeCasted && diff.CanHit(CurrentTarget) && !linkens &&
                    this.Spectre.DiffusalHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    diff.UseAbility(CurrentTarget);
                    await Task.Delay(diff.GetCastDelay(), token);
                }

                var urn = this.Spectre.Urn;
                if (urn != null && urn.CanBeCasted && urn.CanHit(CurrentTarget) && !linkens &&
                    urn.Item.CurrentCharges > 1)
                {
                    urn.UseAbility(CurrentTarget);
                    await Task.Delay(urn.GetCastDelay(), token);
                }

                var manta = this.Spectre.Manta;
                if (manta != null && manta.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 &&
                    this.Spectre.MantaHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    manta.UseAbility();
                    await Task.Delay(manta.GetCastDelay(), token);
                }

                var nullifier = this.Spectre.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.Spectre.NullifierHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    nullifier.UseAbility(CurrentTarget);
                    await Task.Delay(nullifier.GetCastDelay(), token);
                }
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

        private async Task RealityUsage(CancellationToken token)
        {
            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible)
            {
                return;
            }

            Unit target = this.CurrentTarget;

            float distance = this.Owner.Distance2D(this.CurrentTarget);

            HauntIllusions = EntityManager<Unit>.Entities.Where(x => x.Name == this.Owner.Name && x.IsAlive && x.IsIllusion && !x.IsInvulnerable()
            && x.IsAlly(this.Owner) && x.HasModifier("modifier_spectre_haunt")).ToList();


            if (HauntIllusions.Count == 0)
            {
                return;
            }

            var reUseRealityCondition = this.Owner.Distance2D(this.CurrentTarget) > 300 &&
                                        HauntIllusions.Any(x => x.Distance2D(this.CurrentTarget) < 300);
            if (this.Spectre.Config.General.ComboKey && reUseRealityCondition)
            {
                this.Spectre.Reality.UseAbility(this.CurrentTarget.Position);
                await Task.Delay(200, token);
            }
            await Task.Delay(125, token);
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Spectre.Config.General.ComboKey.Value.Active)
            {
                await Task.Delay(250, token);
                return;
            }

            if (this.Spectre.ShadowStep == null && this.Owner.HasAghanimsScepter())
            {
                this.Spectre.ShadowStep = this.Owner.Spellbook.SpellF;
            }

            var illusions =
                EntityManager<Unit>.Entities.Where(
                        x => x.IsValid && x.IsAlive && x.IsIllusion && x.Team == this.Owner.Team && x.IsControllable && !x.HasModifier("modifier_spectre_haunt"))
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

        protected async Task BreakLinken(CancellationToken token)
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<string, uint>> breakerChanger = new List<KeyValuePair<string, uint>>();

                    if (this.CurrentTarget.IsLinkensProtected())
                    {
                        breakerChanger = this.Spectre.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Spectre.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Spectre.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Spectre.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Spectre.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Spectre.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Spectre.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Spectre.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Spectre.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Spectre.DiffusalBlade;
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