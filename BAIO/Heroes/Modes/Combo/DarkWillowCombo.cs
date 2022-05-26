﻿using Ensage.Common.Extensions;

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

    public class DarkWillowCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DarkWillow DarkWillow;

        public DarkWillowCombo(DarkWillow hero)
            : base(hero)
        {
            this.DarkWillow = hero;
        }

        public override void OrbwalkToTarget()
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid && !this.CurrentTarget.IsEthereal() && !this.CurrentTarget.IsInvulnerable() &&
                (!this.DarkWillow.Config.General.KiteMode || this.Owner.Distance2D(this.CurrentTarget) <= CurrentAttackRange))
            {
                this.DarkWillow.Context.Orbwalker.Active.OrbwalkTo(this.CurrentTarget);
            }
            else
            {
                this.DarkWillow.Context.Orbwalker.Active.OrbwalkTo(null);
            }
        }

        private float CurrentAttackRange
        {
            get
            {
                return this.DarkWillow.Owner.HasModifier(this.DarkWillow.ShadowRealm.ModifierName)
                    ? this.DarkWillow.ShadowRealm.CastRange
                    : this.Owner.AttackRange;
            }
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = 1500;

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible || !this.DarkWillow.ShouldHit(this.CurrentTarget))
            {
                this.DarkWillow.Context.Orbwalker.Active.OrbwalkTo(null);
            }

            if (this.CurrentTarget == null || this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            if (this.DarkWillow.RealmStatus == ShadowRealmStatus.Deactivated)
            {
                if (this.DarkWillow.ShadowRealm.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= this.DarkWillow.ShadowRealm.CastRange - 200)
                {
                    if (this.DarkWillow.ShadowRealm.UseAbility())
                    {
                        await Task.Delay(this.DarkWillow.ShadowRealm.GetCastDelay(this.CurrentTarget) + 50, token);
                    }
                }
            }

            var distance = this.CurrentTarget.Distance2D(this.Owner);
            var hurricanePike = this.DarkWillow.HurricanePike;
            if (hurricanePike != null && !linkens && this.Owner.GetModifierByName(this.DarkWillow.ShadowRealm.ModifierName) != null)
            {
                if (this.Owner.HasModifier(hurricanePike.ModifierName))
                {
                    this.Owner.Attack(this.CurrentTarget);
                    await Task.Delay(125, token);
                    return;
                }

                if (hurricanePike.CanBeCasted && hurricanePike.CanHit(this.CurrentTarget) &&
                    (this.DarkWillow.RealmStatus == ShadowRealmStatus.Deactivated) &&
                    (distance <= this.DarkWillow.ShadowRealm.CastRange))
                {
                    hurricanePike.UseAbility(this.CurrentTarget);
                    await Task.Delay(hurricanePike.GetCastDelay(this.CurrentTarget), token);
                    return;
                }
            }

            if (this.DarkWillow.CursedCrown.CanBeCasted && this.DarkWillow.CursedCrown.CanHit(this.CurrentTarget)
                && this.Owner.Distance2D(this.CurrentTarget) <= this.DarkWillow.CursedCrown.CastRange)
            {
                this.DarkWillow.CursedCrown.UseAbility(this.CurrentTarget);
                await Task.Delay(this.DarkWillow.ShadowRealm.GetCastDelay(this.CurrentTarget), token);
            }

            var eul = this.DarkWillow.Euls;
            var shouldEul = !this.Owner.AghanimState() ||
                            (this.Owner.AghanimState() && this.Owner.GetModifierByName(this.DarkWillow.ShadowRealm.ModifierName) == null);


            if (eul != null && !linkens && shouldEul &&
                this.CurrentTarget.GetModifierByName(this.DarkWillow.CursedCrown.TargetModifierName) != null &&
                (this.CurrentTarget.GetModifierByName(this.DarkWillow.CursedCrown.TargetModifierName).RemainingTime >= 2.8f &&
                 this.CurrentTarget.GetModifierByName(this.DarkWillow.CursedCrown.TargetModifierName).RemainingTime <= 3.2f))
            {
                eul.UseAbility(this.CurrentTarget);
                await Task.Delay(eul.GetCastDelay(this.CurrentTarget), token);
            }

            var bedlam = this.DarkWillow.Bedlam;
            if (bedlam.CanBeCasted && !this.CurrentTarget.IsMagicImmune() &&
                this.Owner.Distance2D(this.CurrentTarget) <= 300 && !this.CurrentTarget.IsInvul())
            {
                if (bedlam.UseAbility())
                {
                    await Task.Delay(bedlam.GetCastDelay(this.CurrentTarget), token);
                }
            }

            var brambleMaze = this.DarkWillow.BrambleMaze;
            if (brambleMaze.CanBeCasted && brambleMaze.CanHit(this.CurrentTarget) &&
                this.Owner.Distance2D(this.CurrentTarget) < brambleMaze.CastRange)
            {
                if (brambleMaze.UseAbility(this.CurrentTarget.InFront(150)))
                {
                    await Task.Delay(brambleMaze.GetCastDelay(this.CurrentTarget), token);
                }
            }

            var mom = this.DarkWillow.Mom;
            if ((mom != null) && mom.CanBeCasted && !this.DarkWillow.ShadowRealm.CanBeCasted)
            {
                mom.UseAbility();
                await Task.Delay(mom.GetCastDelay(), token);
            }

            var nullifier = this.DarkWillow.Nullifier;
            if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                this.DarkWillow.NullifierHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                nullifier.UseAbility(CurrentTarget);
                await Task.Delay(nullifier.GetCastDelay(), token);
            }

            var hex = this.DarkWillow.Sheepstick;
            if (hex != null && hex.CanBeCasted && hex.CanHit(CurrentTarget) && !linkens &&
                this.DarkWillow.HexHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                hex.UseAbility(CurrentTarget);
                await Task.Delay(hex.GetCastDelay(), token);
            }

            var veil = this.DarkWillow.VeilOfDiscord;
            if (veil != null && veil.CanBeCasted && veil.CanHit(CurrentTarget))
            {
                veil.UseAbility(CurrentTarget.Position);
                await Task.Delay(veil.GetCastDelay(), token);
            }

            var bloodthorn = this.DarkWillow.BloodThorn;
            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens)
            {
                bloodthorn.UseAbility(CurrentTarget);
                await Task.Delay(bloodthorn.GetCastDelay(), token);
            }

            var orchid = this.DarkWillow.Orchid;
            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens)
            {
                orchid.UseAbility(CurrentTarget);
                await Task.Delay(orchid.GetCastDelay(), token);
            }

            var satanic = this.DarkWillow.Satanic;
            if ((satanic != null) && satanic.CanBeCasted && (this.Owner.HealthPercent() < 0.35f))
            {
                satanic.UseAbility();
                await Task.Delay(satanic.GetCastDelay(), token);
            }

            var mjollnir = this.DarkWillow.Mjollnir;
            if (mjollnir != null && mjollnir.CanBeCasted)
            {
                mjollnir.UseAbility(this.Owner);
                await Task.Delay(mjollnir.GetCastDelay(), token);
            }

            var shouldHit = this.DarkWillow.ShouldHit(this.CurrentTarget);
            if (shouldHit)
            {
                this.OrbwalkToTarget();
                return;
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
                        breakerChanger = this.DarkWillow.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.DarkWillow.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.DarkWillow.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.DarkWillow.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.DarkWillow.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.DarkWillow.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.DarkWillow.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.DarkWillow.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.DarkWillow.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.DarkWillow.DiffusalBlade;
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
