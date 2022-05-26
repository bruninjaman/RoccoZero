using Ensage.Common.Extensions;

namespace BAIO.Heroes.Modes.Combo
{
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
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Geometry;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Prediction;
    using log4net;
    using PlaySharp.Toolkit.Logging;
    using SharpDX;

    internal class ClockwerkCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Clockwerk Clockwerk;

        private Vector3 hookPredictionPos;

        private float hookStartCastTime;

        private bool hookHit;

        private readonly IUpdateHandler ultiUpdateHandler;

        public ClockwerkCombo(Clockwerk hero)
            : base(hero)
        {
            this.Clockwerk = hero;
            Entity.OnBoolPropertyChange += this.OnHookCast;
            this.ultiUpdateHandler = UpdateManager.Subscribe(this.HookHitCheck, 0, false);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, Clockwerk.Ulti.CastRange * 1.1f);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Clockwerk.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            var atos = this.Clockwerk.RodOfAtos;
            if (Clockwerk.Ulti.CanBeCasted && Clockwerk.Ulti.CanHit(this.CurrentTarget) &&
                !this.CurrentTarget.IsInvulnerable())
            {
                if (atos?.CanBeCasted == true && atos.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsStunned() && !linkens &&
                    !this.CurrentTarget.IsRooted())
                {
                    var hookPreAtosInput = Clockwerk.Ulti.GetPredictionInput(this.CurrentTarget);
                    var hookPreAtosOutput = Clockwerk.Ulti.GetPredictionOutput(hookPreAtosInput);

                    if ((hookPreAtosOutput.HitChance != HitChance.OutOfRange) &&
                        (hookPreAtosOutput.HitChance != HitChance.Collision))
                    {
                        atos.UseAbility(this.CurrentTarget);
                        await Task.Delay(atos.GetHitTime(this.CurrentTarget), token);
                    }
                }

                var hookInput = Clockwerk.Ulti.GetPredictionInput(this.CurrentTarget);
                var hookOutput = Clockwerk.Ulti.GetPredictionOutput(hookInput);

                if (this.CanUltiHit(hookOutput))
                {
                    this.hookPredictionPos = hookOutput.UnitPosition;
                    Clockwerk.Ulti.UseAbility(this.hookPredictionPos);
                    await Task.Delay(Clockwerk.Ulti.GetHitTime(this.hookPredictionPos), token);
                }
            }

            var insec = this.Clockwerk.Insec.Value.Active;
            if (Clockwerk.Cogs.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 170)
            {
                if (insec && (this.CurrentTarget.IsStunned() || this.CurrentTarget.IsRooted()))
                {
                    var teammates =
                        EntityManager<Hero>.Entities.Where(
                                x => x.IsValid && x.Team == this.Owner.Team && x.Distance2D(this.Owner) <= 800)
                            .OrderBy(x => x.Distance2D(this.Owner)).FirstOrDefault();

                    if (teammates != null)
                    {
                        var pos = teammates.NetworkPosition.Extend(CurrentTarget.NetworkPosition,
                            teammates.Distance2D(CurrentTarget) + 400);
                        this.Owner.Move(pos);
                        await Task.Delay(GetMoveDelay(pos), token);
                        this.Clockwerk.Cogs.UseAbility();
                        await Task.Delay(Clockwerk.Cogs.GetCastDelay(), token);
                    }
                }
                else
                {
                    this.Clockwerk.Cogs.UseAbility();
                    await Task.Delay(Clockwerk.Cogs.GetCastDelay(), token);
                }
            }

            if (Clockwerk.BatteryAssault.CanBeCasted &&
                (this.Clockwerk.IsHookModifier || this.Owner.Distance2D(CurrentTarget) <= 200))
            {
                this.Clockwerk.BatteryAssault.UseAbility();
                await Task.Delay(Clockwerk.BatteryAssault.GetCastDelay(), token);
            }
            if (!insec || !Clockwerk.Cogs.CanBeCasted)
            {
                var bladeMail = this.Clockwerk.BladeMail;
                if (bladeMail != null && bladeMail.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 && this.Clockwerk.BladeMailUsage)
                {
                    bladeMail.UseAbility();
                    await Task.Delay(bladeMail.GetCastDelay(), token);
                }

                var lotusOrb = this.Clockwerk.Lotus;
                if (lotusOrb != null && lotusOrb.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 && this.Clockwerk.LotusUsage)
                {
                    lotusOrb.UseAbility(this.Owner);
                    await Task.Delay(lotusOrb.GetCastDelay(), token);
                }

                var solar = this.Clockwerk.SolarCrest;
                if (solar != null && solar.CanBeCasted && solar.CanHit(CurrentTarget))
                {
                    solar.UseAbility(this.CurrentTarget);
                    await Task.Delay(solar.GetCastDelay(), token);
                }

                var vessel = this.Clockwerk.Vessel;
                var urn = this.Clockwerk.Urn;
                if ((urn?.CanBeCasted == true && this.Clockwerk.UrnHeroes.Value.IsEnabled(CurrentTarget.Name)
                    || vessel?.CanBeCasted == true && this.Clockwerk.VesselHeroes.Value.IsEnabled(CurrentTarget.Name))
                    && (this.Clockwerk.IsHookModifier || this.Owner.Distance2D(this.CurrentTarget) < 300) && !linkens)
                {
                    urn?.UseAbility(this.CurrentTarget);
                    vessel?.UseAbility(this.CurrentTarget);
                }

                var nullifier = this.Clockwerk.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.Clockwerk.NullifierHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    nullifier.UseAbility(CurrentTarget);
                    await Task.Delay(nullifier.GetCastDelay(), token);
                }

                var halberd = this.Clockwerk.HeavensHalberd;
                if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
                    this.Clockwerk.HalberdHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    halberd.UseAbility(CurrentTarget);
                    await Task.Delay(halberd.GetCastDelay(), token);
                }

                var orchid = this.Clockwerk.Orchid;
                if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
                    this.Clockwerk.OrchidHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    orchid.UseAbility(CurrentTarget);
                    await Task.Delay(orchid.GetCastDelay(), token);
                }

                var bt = this.Clockwerk.BloodThorn;
                if (bt != null && bt.CanBeCasted && bt.CanHit(CurrentTarget) && !linkens &&
                    this.Clockwerk.OrchidHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    bt.UseAbility(CurrentTarget);
                    await Task.Delay(bt.GetCastDelay(), token);
                }
            }
            this.OrbwalkToTarget();
        }

        private int GetMoveDelay(Vector3 pos)
        {
            return (int) (((this.Owner.Distance2D(pos) / this.Owner.MovementSpeed) * 1000.0) + Game.Ping);
        }

        private bool CanUltiHit(PredictionOutput output)
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

            return output.HitChance >= this.Clockwerk.MinimumHookChance;
        }

        private void OnHookCast(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (args.NewValue == args.OldValue || sender != this.Clockwerk.Ulti ||
                args.PropertyName != "m_bInAbilityPhase")
            {
                return;
            }

            if (args.NewValue)
            {
                this.hookHit = true;
                this.hookStartCastTime = Game.RawGameTime;
                this.ultiUpdateHandler.IsEnabled = true;
            }
            else
            {
                this.hookHit = false;
                this.ultiUpdateHandler.IsEnabled = false;
            }
        }

        private void HookHitCheck()
        {
            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible)
            {
                return;
            }

            var hook = this.Clockwerk.Ulti;
            var input = hook.GetPredictionInput(this.CurrentTarget);
            input.Delay = Math.Max((this.hookStartCastTime - Game.RawGameTime) + hook.CastPoint, 0);
            var output = hook.GetPredictionOutput(input);

            if (this.hookPredictionPos.Distance2D(output.UnitPosition) > hook.Radius || !this.CanUltiHit(output))
            {
                this.Owner.Stop();
                this.Cancel();
                this.ultiUpdateHandler.IsEnabled = false;
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
                        breakerChanger = this.Clockwerk.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Clockwerk.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Clockwerk.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Clockwerk.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Clockwerk.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Clockwerk.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Clockwerk.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Clockwerk.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Clockwerk.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Clockwerk.DiffusalBlade;
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