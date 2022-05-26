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
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;
using log4net;
using PlaySharp.Toolkit.Logging;
using SharpDX;
using EntityExtensions = Ensage.Common.Extensions.EntityExtensions;
using UnitExtensions = Ensage.Common.Extensions.UnitExtensions;

namespace BAIO.Heroes.Modes.Combo
{
    internal class SnapfireCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Snapfire Snapfire;
        public TaskHandler Handlee { get; private set; }


        public SnapfireCombo(Snapfire hero)
            : base(hero)
        {
            this.Snapfire = hero;
            this.Handlee = UpdateManager.Run(this.OnUpdate);
        }

        protected override void OnDeactivate()
        {
            this.Handlee.Cancel();
            base.OnDeactivate();
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, Snapfire.Kiss.CastRange * 1.1f);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Snapfire.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            try
            {
                if (!Ensage.SDK.Extensions.UnitExtensions.HasModifier(this.Owner, "modifier_snapfire_mortimer_kisses"))
                {
                    var linkens = UnitExtensions.IsLinkensProtected(this.CurrentTarget);
                    await BreakLinken(token);

                    var veil = this.Snapfire.VeilOfDiscord;
                    if (veil != null && veil.CanBeCasted && veil.CanHit(CurrentTarget))
                    {
                        veil.UseAbility(CurrentTarget.Position);
                        await Task.Delay(veil.GetCastDelay(), token);
                    }

                    var hex = this.Snapfire.Sheepstick;
                    if (hex != null && hex.CanBeCasted && hex.CanHit(CurrentTarget) && !linkens &&
                        this.Snapfire.HexHeroes.Value.IsEnabled(CurrentTarget.Name))
                    {
                        hex.UseAbility(CurrentTarget);
                        await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                    }

                    var atos = this.Snapfire.RodOfAtos;
                    if (atos != null && atos.CanBeCasted && atos.CanHit(CurrentTarget) && !linkens)
                    {
                        atos.UseAbility(CurrentTarget);
                        await Task.Delay(atos.GetCastDelay(CurrentTarget), token);
                    }

                    var bloodthorn = this.Snapfire.BloodThorn;
                    if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens && this.Snapfire.OrchidHeroes.Value.IsEnabled(this.CurrentTarget.Name))
                    {
                        bloodthorn.UseAbility(CurrentTarget);
                        await Task.Delay(bloodthorn.GetCastDelay(CurrentTarget), token);
                    }

                    var orchid = this.Snapfire.Orchid;
                    if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens && this.Snapfire.OrchidHeroes.Value.IsEnabled(this.CurrentTarget.Name))
                    {
                        orchid.UseAbility(CurrentTarget);
                        await Task.Delay(orchid.GetCastDelay(CurrentTarget), token);
                    }

                    var scatterblast = this.Snapfire.Scatterblast;
                    if (scatterblast.CanBeCasted && EntityExtensions.Distance2D(this.Owner, this.CurrentTarget) <= scatterblast.CastRange)
                    {
                        var input = scatterblast.GetPredictionInput(this.CurrentTarget);
                        var output = this.Snapfire.Context.Prediction.GetPrediction(input);

                        if (output.HitChance >= HitChance.Medium)
                        {
                            scatterblast.UseAbility(output.CastPosition);
                            await Task.Delay(scatterblast.GetCastDelay(output.CastPosition), token);
                        }
                    }

                    var cookie = this.Snapfire.Cookie;
                    if (cookie.CanBeCasted)
                    {
                        var allies = EntityManager<Unit>.Entities.Where(x =>
                            x.IsValid && cookie.CanHit(x) && x.Team == this.Owner.Team && x.Distance2D(this.Owner) <= cookie.CastRange &&
                            x.Distance2D(this.CurrentTarget) <= 600 && x.Distance2D(this.CurrentTarget) >= 150).OrderBy(x => x.Distance2D(this.Owner)).ToList();
                        foreach (var ally in allies)
                        {
                            if (Ensage.Common.Extensions.UnitExtensions.InFront(ally, 450).Distance2D(this.CurrentTarget) >= 200)
                            {
                                continue;
                            }

                            var input = new PredictionInput(this.Owner, ally, ally == this.Owner ? 0.3f : this.Owner.Distance2D(ally) / 1000, 1000, 450, 350);
                            var output = this.Snapfire.Context.Prediction.GetPrediction(input);

                            if (output.HitChance >= HitChance.Medium)
                            {
                                if (cookie.UseAbility(ally))
                                {
                                    await Task.Delay(cookie.GetCastDelay(ally), token);
                                    break;
                                }
                            }
                        }
                    }

                    var E = this.Snapfire.Shredder;
                    if (E.CanBeCasted && this.CurrentTarget.Distance2D(this.Owner) <= 
                        (this.Owner.AttackRange + this.Snapfire.Shredder.Ability.GetAbilitySpecialData("attack_range_bonus")) - 150)
                    {
                        if (E.UseAbility())
                        {
                            await Task.Delay(E.GetCastDelay(), token);
                        }
                    }

                    var nullifier = this.Snapfire.Nullifier;
                    if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                        this.Snapfire.NullifierHeroes.Value.IsEnabled(CurrentTarget.Name))
                    {
                        nullifier.UseAbility(CurrentTarget);
                        await Task.Delay(nullifier.GetCastDelay(), token);
                    }

                    var solarCrest = this.Snapfire.SolarCrest;
                    if (solarCrest != null && solarCrest.CanBeCasted && solarCrest.CanHit(CurrentTarget) && !linkens)
                    {
                        nullifier.UseAbility(CurrentTarget);
                        await Task.Delay(nullifier.GetCastDelay(), token);
                    }

                    //if (Ensage.SDK.Extensions.UnitExtensions.HasModifier(this.Owner, "modifier_snapfire_lil_shredder_buff"))
                    //{
                    //    this.Snapfire.Context.Orbwalker.Attack(this.CurrentTarget);
                    //    return;
                    //}
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

            //if (!Ensage.SDK.Extensions.UnitExtensions.HasModifier(this.Owner, "modifier_snapfire_lil_shredder_buff"))
            //{
                this.OrbwalkToTarget();
            //}
        }


        private async Task OnUpdate(CancellationToken token)
        {
            var target = this.Context.TargetSelector.Active.GetTargets().FirstOrDefault();
            if (Game.IsPaused || !this.Owner.IsAlive || target == null)
            {
                await Task.Delay(250, token);
                return;
            }

            //if (!this.Snapfire.Kiss.CanBeCasted || UltiTarget == null)
            //{
            //    if (this.Snapfire.UltiCombo.Value.Active)
            //    {
            //        this.Context.Orbwalker.OrbwalkTo(null);
            //        return;
            //    }
            //    //await Task.Delay(250, token);
            //    return;
            //}

            if (this.Snapfire.UltiCombo.Value.Active && target != null && target.Distance2D(this.Owner) >= 600
                && !Ensage.SDK.Extensions.UnitExtensions.HasModifier(this.Owner,"modifier_snapfire_mortimer_kisses"))
            {
                var input = new PredictionInput(this.Owner, target, this.Snapfire.Kiss.GetCastDelay(target) / 1000f, this.Snapfire.Kiss.Ability.GetAbilitySpecialData("projectile_speed"), 3000, 250);
                var output = this.Snapfire.Context.Prediction.GetPrediction(input);
                if (target.Distance2D(this.Owner) <= this.Snapfire.Kiss.CastRange)
                {
                    if (this.Snapfire.Kiss.CanBeCasted && !Ensage.SDK.Extensions.UnitExtensions.IsMagicImmune(target))
                    {
                        if (this.Snapfire.Kiss.UseAbility(output.CastPosition))
                        {
                            await Task.Delay(this.Snapfire.Kiss.GetCastDelay(output.CastPosition), token);
                        }
                    }
                }
            }
            if (this.Snapfire.UltiCombo.Value.Active && target != null && Ensage.SDK.Extensions.UnitExtensions.HasModifier(this.Owner, "modifier_snapfire_mortimer_kisses"))
            {
                var input = new PredictionInput(this.Owner, target, this.Snapfire.Kiss.GetCastDelay(target) / 1000f, this.Snapfire.Kiss.Ability.GetAbilitySpecialData("projectile_speed"), 3000, 250);
                var output = this.Snapfire.Context.Prediction.GetPrediction(input);
                if (lastCastPosition.Distance2D(output.CastPosition) > 25)
                {
                    if (this.Owner.Move(output.CastPosition))
                    {
                        lastCastPosition = output.CastPosition;
                        await Task.Delay(100, token);
                        return;
                    }
                }
            }

            await Task.Delay(100, token);
        }

        private Vector3 lastCastPosition;

        protected async Task BreakLinken(CancellationToken token)
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<string, uint>> breakerChanger = new List<KeyValuePair<string, uint>>();

                    if (UnitExtensions.IsLinkensProtected(this.CurrentTarget))
                    {
                        breakerChanger = this.Snapfire.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Snapfire.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Snapfire.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Snapfire.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Snapfire.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Snapfire.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Snapfire.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Snapfire.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Snapfire.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Snapfire.DiffusalBlade;
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