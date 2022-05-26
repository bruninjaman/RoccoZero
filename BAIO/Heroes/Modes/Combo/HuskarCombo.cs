using System;
using System.Collections.Generic;
using Ensage.Common.Extensions;
using Ensage.SDK.Handlers;

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

    internal class HuskarCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Huskar Huskar;

        private TaskHandler innerFireHandler;

        public HuskarCombo(Huskar hero)
            : base(hero)
        {
            this.Huskar = hero;
            this.innerFireHandler = UpdateManager.Run(this.OnUpdate);
        }

        protected override void OnDeactivate()
        {
            this.innerFireHandler.Cancel();

            base.OnDeactivate();
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                //if (this.Huskar.FireSpear.Enabled)
                //{
                //    this.Huskar.FireSpear.Enabled = false;
                //}

                if (!this.Huskar.IsArmletEnabled)
                {
                    var armlet = this.Huskar.Armlet;
                    if (armlet != null && !this.Owner.IsInvisible())
                    {
                        armlet.Enabled = false;
                        await Task.Delay(armlet.GetCastDelay(), token);
                    }
                }

                this.Huskar.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            if (this.Huskar.Ulti.CanBeCasted && this.Huskar.Ulti.CanHit())
            {
                this.Huskar.Ulti.UseAbility(CurrentTarget);
                await Task.Delay(Huskar.Ulti.GetCastDelay(), token);
            }

            if (!this.Huskar.IsArmletEnabled)
            {
                var armlet = this.Huskar.Armlet;
                if (armlet != null && !this.Owner.IsInvisible())
                {
                    if (CurrentTarget != null)
                    {
                        armlet.Enabled = true;
                    }
                    await Task.Delay(armlet.GetCastDelay(), token);
                }
                await Task.Delay(100, token);
            }

            var distance = this.CurrentTarget.Distance2D(this.Owner);
            var hurricanePike = this.Huskar.HurricanePike;
            var ultModif = this.Owner.HasModifier("modifier_huskar_life_break_charge");

            if (this.CurrentTarget != null && hurricanePike != null && ShouldCastPike() && !ultModif && !linkens)
            {
                if (this.Owner.HasModifier(hurricanePike.ModifierName) && this.CurrentTarget.IsVisible)
                {
                    if (!this.Huskar.FireSpear.Ability.IsAutoCastEnabled)
                    {
                        this.Huskar.FireSpear.Ability.ToggleAutocastAbility();
                    }
                    this.Owner.Attack(this.CurrentTarget);
                    await Task.Delay(125, token);
                    return;
                }
                else if (!this.Owner.HasModifier(hurricanePike.ModifierName) && this.Huskar.FireSpear.Ability.IsAutoCastEnabled)
                {
                    this.Huskar.FireSpear.Ability.ToggleAutocastAbility();
                    await Task.Delay(50, token);
                }

                if (hurricanePike.CanBeCasted && hurricanePike.CanHit(this.CurrentTarget))
                {
                    hurricanePike.UseAbility(this.CurrentTarget);
                    await Task.Delay(hurricanePike.GetCastDelay(this.CurrentTarget), token);
                    return;
                }
            }

            var halberd = this.Huskar.HeavensHalberd;
            if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
                this.Huskar.HalberdHeroes.Value.IsEnabled(CurrentTarget?.Name))
            {
                halberd.UseAbility(CurrentTarget);
                await Task.Delay(halberd.GetCastDelay(), token);
            }

            var solarCrest = this.Huskar.SolarCrest;
            if (solarCrest != null && solarCrest.CanBeCasted && solarCrest.CanHit(CurrentTarget) &&
                this.Huskar.MedallionHeroes.Value.IsEnabled(CurrentTarget?.Name))
            {
                solarCrest.UseAbility(CurrentTarget);
                await Task.Delay(solarCrest.GetCastDelay(), token);
            }

            var medallion = this.Huskar.Medallion;
            if (medallion != null && medallion.CanBeCasted && medallion.CanHit(CurrentTarget) &&
                this.Huskar.MedallionHeroes.Value.IsEnabled(CurrentTarget?.Name))
            {
                medallion.UseAbility(CurrentTarget);
                await Task.Delay(medallion.GetCastDelay(), token);
            }

            var bloodThorn = this.Huskar.BloodThorn;
            if (bloodThorn != null && bloodThorn.CanBeCasted && bloodThorn.CanHit(CurrentTarget) && !linkens &&
                this.Huskar.BloodthornHeroes.Value.IsEnabled(CurrentTarget?.Name))
            {
                bloodThorn.UseAbility(CurrentTarget);
                await Task.Delay(bloodThorn.GetCastDelay(), token);
            }

            var mom = this.Huskar.Mom;
            if (mom != null && mom.CanBeCasted && !this.Huskar.Ulti.CanBeCasted)
            {
                mom.UseAbility();
                await Task.Delay(mom.GetCastDelay(), token);
            }

            var satanic = this.Huskar.Satanic;
            if (satanic != null && satanic.CanBeCasted &&
                (this.Owner.HealthPercent() < this.Huskar.SatanicPercent.Item.GetValue<Slider>().Value / 100f))
            {
                satanic.UseAbility();
                await Task.Delay(satanic.GetCastDelay(), token);
            }

            var mjollnir = this.Huskar.Mjollnir;
            if (mjollnir != null && mjollnir.CanBeCasted)
            {
                mjollnir.UseAbility(this.Owner);
                await Task.Delay(mjollnir.GetCastDelay(), token);
            }

            var useArrows = this.Huskar.IsFireSpearGucci(this.CurrentTarget);
            if (useArrows)
            {
                if (this.Huskar.FireSpear.CanBeCasted && this.Huskar.FireSpear.CanHit(CurrentTarget) /*&&
                    !this.Huskar.FireSpear.Enabled*/)
                {
                    //this.Huskar.FireSpear.Enabled = true;
                    await Huskar.UseFireSpear(this.CurrentTarget, token);
                }
            }

            this.OrbwalkToTarget();
        }

        private bool ShouldCastPike()
        {
            if (Game.IsPaused || !this.Owner.IsAlive || this.Huskar.HurricanePike == null || this.CurrentTarget == null)
            {
                return false;
            }

            if (this.Huskar.HurricaneHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                return true;
            }

            if (this.CurrentTarget.FindModifier("modifier_huskar_burning_spear_debuff") != null)
            {
                var stacksOnTarget = this.CurrentTarget.FindModifier("modifier_huskar_burning_spear_debuff").StackCount;
                if (stacksOnTarget > 0)
                {
                    return this.CurrentTarget.Health + (this.CurrentTarget.HealthRegeneration * 6) <= this.Huskar.FireSpear.GetTotalDamage(this.CurrentTarget) * (2 + stacksOnTarget);
                }
            }
            return this.CurrentTarget.Health + this.CurrentTarget.HealthRegeneration * 6 <= this.Huskar.FireSpear.GetTotalDamage(this.CurrentTarget) * 2;
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || !this.Huskar.InnerFire.CanBeCasted)
            {
                await Task.Delay(100, token);
                return;
            }

            var targets = EntityManager<Hero>.Entities.Where(x =>
                x != null && x.IsValid && x.Distance2D(this.Owner) <= 400 && x.Team != this.Owner.Team &&
                x.IsAlive && x.IsVisible
                && x.UnitState != UnitState.MagicImmune).ToList();

            if (!targets.Any())
            {
                return;
            }

            if (Huskar.InnerFire != null && this.Huskar.InnerFire.CanBeCasted &&
                (((float)this.Owner.Health / this.Owner.MaximumHealth) * 100f) <= this.Huskar.InnerFireThreshold.Value.Value)
            {
                this.Huskar.InnerFire.UseAbility();
                await Task.Delay(this.Huskar.InnerFire.GetCastDelay(), token);
            }

            await Task.Delay(25, token);
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
                        breakerChanger = this.Huskar.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Huskar.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Huskar.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Huskar.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Huskar.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Huskar.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Huskar.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Huskar.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Huskar.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Huskar.DiffusalBlade;
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