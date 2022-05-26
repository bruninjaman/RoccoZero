using Ensage;
using Ensage.Common.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Prediction;

namespace BAIO.Heroes.Modes.Combo
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Ensage.SDK.Extensions;
    using log4net;
    using PlaySharp.Toolkit.Logging;
    using BAIO.Heroes;
    using BAIO.Modes;
    using BAIO.Heroes.Base;

    public class DrowRangerCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DrowRanger DrowRanger;
        private TaskHandler ArrowHandler;


        public DrowRangerCombo(DrowRanger hero)
            : base(hero)
        {
            this.DrowRanger = hero;
            this.ArrowHandler = UpdateManager.Run(this.OnUpdate);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            this.ArrowHandler?.Cancel();
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, this.Owner.AttackRange * 1.5f);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.DrowRanger.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            if (this.Owner.IsChanneling())
            {
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            var distance = this.CurrentTarget.Distance2D(this.Owner);
            var hurricanePike = this.DrowRanger.HurricanePike;
            if (hurricanePike != null && !linkens)
            {
                if (this.Owner.HasModifier(hurricanePike.ModifierName))
                {
                    this.Owner.Attack(this.CurrentTarget);
                    await Task.Delay(125, token);
                    return;
                }

                if (hurricanePike.CanBeCasted && hurricanePike.CanHit(this.CurrentTarget) &&
                    (this.DrowRanger.UltiStatus == UltiStatus.Deactivated) &&
                    (distance <= this.DrowRanger.Marksmanship.Radius))
                {
                    hurricanePike.UseAbility(this.CurrentTarget);
                    await Task.Delay(hurricanePike.GetCastDelay(this.CurrentTarget), token);
                    return;
                }
            }

            if (this.DrowRanger.WaveOfSilence.CanHit(this.CurrentTarget) && (this.DrowRanger.UltiStatus == UltiStatus.Deactivated))
            {
                var manta = this.DrowRanger.Manta;
                var usedManta = false;
                if (this.Owner.IsSilenced() && (manta != null) &&
                    manta.CanBeCasted &&
                    (this.DrowRanger.WaveOfSilence.Ability.Cooldown == 0))
                {
                    manta.UseAbility();
                    await Task.Delay(manta.GetCastDelay(), token);
                    usedManta = true;
                }

                if (usedManta || this.DrowRanger.WaveOfSilence.CanBeCasted)
                {
                    this.DrowRanger.WaveOfSilence.UseAbility(this.CurrentTarget);
                    await Task.Delay(this.DrowRanger.WaveOfSilence.GetCastDelay(this.CurrentTarget), token);
                    return;
                }
            }
            else if (this.DrowRanger.WaveOfSilence.CanBeCasted && this.DrowRanger.WaveOfSilence.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsMagicImmune() &&
                     this.DrowRanger.SilenceHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                if (this.DrowRanger.WaveOfSilence.UseAbility(this.CurrentTarget))
                {
                    await Task.Delay(this.DrowRanger.WaveOfSilence.GetCastDelay(this.CurrentTarget), token);
                }
            }

            var multiShot = this.DrowRanger.Multishot;
            if (multiShot.CanHit(this.CurrentTarget) && multiShot.CanBeCasted() &&
                this.Owner.Distance2D(this.CurrentTarget) <= this.Owner.AttackRange * 1.5)
            {
                var input = new PredictionInput(this.Owner, this.CurrentTarget, 0.1f, 1250,
                    this.Owner.AttackRange * 1.5f, 100,
                    PredictionSkillshotType.SkillshotLine);
                var output = this.DrowRanger.Context.Prediction.GetPrediction(input);

                if (output.HitChance >= HitChance.Medium)
                {
                    if (multiShot.UseAbility(output.CastPosition))
                    {
                        await Task.Delay((int)(1000f), token);
                    }
                }
            }

            var mom = this.DrowRanger.Mom;
            if ((mom != null) && mom.CanBeCasted && !this.DrowRanger.WaveOfSilence.CanBeCasted)
            {
                mom.UseAbility();
                await Task.Delay(mom.GetCastDelay(), token);
            }

            var nullifier = this.DrowRanger.Nullifier;
            if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                this.DrowRanger.NullifierHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                nullifier.UseAbility(CurrentTarget);
                await Task.Delay(nullifier.GetCastDelay(), token);
            }

            var hex = this.DrowRanger.Sheepstick;
            if (hex != null && hex.CanBeCasted && hex.CanHit(CurrentTarget) && !linkens &&
                this.DrowRanger.HexHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                hex.UseAbility(CurrentTarget);
                await Task.Delay(hex.GetCastDelay(), token);
            }

            var satanic = this.DrowRanger.Satanic;
            if ((satanic != null) && satanic.CanBeCasted && (this.Owner.HealthPercent() < 0.35f))
            {
                satanic.UseAbility();
                await Task.Delay(satanic.GetCastDelay(), token);
            }

            var mjollnir = this.DrowRanger.Mjollnir;
            if (mjollnir != null && mjollnir.CanBeCasted && this.DrowRanger.UltiStatus >= UltiStatus.Danger)
            {
                mjollnir.UseAbility(this.Owner);
                await Task.Delay(mjollnir.GetCastDelay(), token);
            }

            var useArrows = this.DrowRanger.ShouldUseFrostArrow(this.CurrentTarget);
            if (useArrows && !this.Owner.HasModifier("modifier_drow_ranger_multishot"))
            {
                await this.DrowRanger.UseFrostArrows(this.CurrentTarget, token);
                this.OrbwalkToTarget();
                return;
            }

            if (!this.Owner.HasModifier("modifier_drow_ranger_multishot"))
            {
                this.OrbwalkToTarget();
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (!this.Owner.HasModifier("modifier_drow_ranger_multishot"))
            {
                return;
            }

            if (this.Owner.FindModifier("modifier_drow_ranger_multishot").RemainingTime <= 0.45f)
            {
                if (this.Owner.Move(Game.MousePosition))
                {
                    await Task.Delay(150, token);
                }
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
                        breakerChanger = this.DrowRanger.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.DrowRanger.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.DrowRanger.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.DrowRanger.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.DrowRanger.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.DrowRanger.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.DrowRanger.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.DrowRanger.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.DrowRanger.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.DrowRanger.DiffusalBlade;
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
