using System;
using System.Collections.Generic;
using Ensage.Common.Extensions;

namespace BAIO.Heroes.Modes.Combo
{
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using BAIO.Modes;
    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using log4net;
    using PlaySharp.Toolkit.Logging;

    internal class SvenCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Sven Sven;

        private static Attribute DefaultAttribute => Attribute.Strength;

        public SvenCombo(Sven hero)
            : base(hero)
        {
            this.Sven = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            var blink = this.Sven.BlinkDagger;
            if (blink != null)
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange * 1.3f);
            }

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Sven.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            var ulti = this.Sven.Ulti;
            var stun = this.Sven.Stun;
            var warcry = this.Sven.Warcry;

            var forceStaff = this.Sven.ForceStaff;
            var forceStaffReady = (forceStaff != null) && forceStaff.CanBeCasted;
            var blinkReady = blink != null && blink.CanBeCasted;
            var distance = this.Owner.Distance2D(this.CurrentTarget);

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            if (blinkReady && this.Owner.Distance2D(this.CurrentTarget) > 600)
            {
                if (this.Sven.UseUltiBefore)
                {
                    if (ulti.CanBeCasted)
                    {
                        ulti.UseAbility();
                        await Task.Delay(ulti.GetCastDelay(), token);
                    }
                    if (warcry.CanBeCasted)
                    {
                        warcry.UseAbility();
                    }
                }

                var blinkPosition = this.CurrentTarget.NetworkPosition.Extend(this.Owner.NetworkPosition, Math.Max(100, distance - blink.CastRange));
                blink.UseAbility(blinkPosition);

                if (stun.CanBeCasted && stun.CanHit(this.CurrentTarget) && blinkPosition.Distance2D(this.CurrentTarget.NetworkPosition) <= stun.CastRange)
                {
                    if (!linkens && this.Sven.Treads != null && this.Sven.Treads.CanBeCasted)
                    {
                        this.Sven.Treads.SwitchAttribute(Attribute.Intelligence);
                        await Task.Delay(80, token);
                        stun.UseAbility(this.CurrentTarget);
                        await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
                        this.Sven.Treads.SwitchAttribute(Attribute.Strength);
                        await Task.Delay(80, token);
                    }
                    else if (!linkens)
                    {
                        stun.UseAbility(this.CurrentTarget);
                        await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
                    }
                }
                else
                {
                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
                }
            }

            if (stun.CanBeCasted && stun.CanHit(this.CurrentTarget))
            {
                if (!linkens && this.Sven.Treads != null && this.Sven.Treads.CanBeCasted)
                {
                    this.Sven.Treads.SwitchAttribute(Attribute.Intelligence);
                    await Task.Delay(100, token);
                    stun.UseAbility(this.CurrentTarget);
                    await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
                    this.Sven.Treads.SwitchAttribute(Attribute.Strength);
                    await Task.Delay(300, token);
                }
                else if (!linkens)
                {
                    stun.UseAbility(this.CurrentTarget);
                    await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
                }
            }

            if (distance <= 400)
            {
                if (ulti.CanBeCasted)
                {
                    ulti.UseAbility();
                    await Task.Delay(ulti.GetCastDelay(), token);
                }
                if (warcry.CanBeCasted)
                {
                    warcry.UseAbility();
                }
            }

            var mom = this.Sven.Mom;
            var cantCast = !ulti.CanBeCasted && !stun.CanBeCasted && !warcry.CanBeCasted;

            if (mom != null && mom.CanBeCasted && cantCast)
            {
                mom.UseAbility();
                await Task.Delay(mom.GetCastDelay(), token);
            }

            var halberd = this.Sven.HeavensHalberd;
            if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
                this.Sven.HalberdHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                halberd.UseAbility(CurrentTarget);
                await Task.Delay(halberd.GetCastDelay(), token);
            }

            var bloodthorn = this.Sven.BloodThorn;
            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
                this.Sven.BtOrchidHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                bloodthorn.UseAbility(CurrentTarget);
                await Task.Delay(bloodthorn.GetCastDelay(), token);
            }

            var orchid = this.Sven.Orchid;
            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
                this.Sven.BtOrchidHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                orchid.UseAbility(CurrentTarget);
                await Task.Delay(orchid.GetCastDelay(), token);
            }

            var bkb = this.Sven.BlackKingBar;
            if (bkb != null && bkb.CanBeCasted && this.Sven.BkbHeroes.Value.IsEnabled(this.CurrentTarget.Name))
            {
                bkb.UseAbility();
                await Task.Delay(bkb.GetCastDelay(), token);
            }

            var mjollnir = this.Sven.Mjollnir;
            if (mjollnir != null && mjollnir.CanBeCasted)
            {
                mjollnir.UseAbility(this.Owner);
                await Task.Delay(mjollnir.GetCastDelay(), token);
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
                        breakerChanger = this.Sven.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Sven.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Sven.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Sven.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Sven.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Sven.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Sven.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Sven.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Sven.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Sven.DiffusalBlade;
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
