using System;
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
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;
using log4net;
using PlaySharp.Toolkit.Logging;
using SharpDX;

namespace BAIO.Heroes.Modes.Combo
{
    internal class AntiMageCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AntiMage AntiMage;


        public AntiMageCombo(AntiMage hero)
            : base(hero)
        {
            this.AntiMage = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, AntiMage.Blink.CastRange * 1.1f);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.AntiMage.Context.Orbwalker.Active.OrbwalkTo(null);
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

                var blink = this.AntiMage.Blink;
                if (blink.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= blink.CastRange &&
                    this.Owner.Distance2D(this.CurrentTarget) >= this.AntiMage.MinimumBlinkRange && this.AntiMage.BlinkHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    var input = new PredictionInput(this.Owner, CurrentTarget, blink.GetCastDelay() / 1000f,
                        float.MaxValue, blink.CastRange, 100, PredictionSkillshotType.SkillshotCircle, false, null, true);
                    var output = this.AntiMage.Context.Prediction.GetPrediction(input);

                    if (output.HitChance >= HitChance.Medium)
                    {
                        if (blink.UseAbility(output.CastPosition))
                        {
                            await Task.Delay(blink.GetCastDelay(), token);
                        }
                    }
                }

                var abyssal = this.AntiMage.AbyssalBlade;
                if (abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
                    this.AntiMage.AbyssalBladeHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    if (abyssal.UseAbility(CurrentTarget))
                    {
                        await Task.Delay(abyssal.GetCastDelay(), token);
                    }
                }

                var manta = this.AntiMage.Manta;
                if (manta != null && manta.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 &&
                    this.AntiMage.MantaHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    if (manta.UseAbility())
                    {
                        await Task.Delay(manta.GetCastDelay(), token);
                    }
                }

                var nullifier = this.AntiMage.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.AntiMage.NullifierHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    if (nullifier.UseAbility(CurrentTarget))
                    {
                        await Task.Delay(nullifier.GetCastDelay(), token);
                    }
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

        protected async Task BreakLinken(CancellationToken token)
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<string, uint>> breakerChanger = new List<KeyValuePair<string, uint>>();

                    if (this.CurrentTarget.IsLinkensProtected())
                    {
                        breakerChanger = this.AntiMage.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.AntiMage.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.AntiMage.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.AntiMage.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.AntiMage.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.AntiMage.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.AntiMage.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.AntiMage.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.AntiMage.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.AntiMage.DiffusalBlade;
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