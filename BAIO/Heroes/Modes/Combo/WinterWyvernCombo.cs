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
using AbilityExtensions = Ensage.SDK.Extensions.AbilityExtensions;

namespace BAIO.Heroes.Modes.Combo
{
    internal class WinterWyvernCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly WinterWyvern WinterWyvern;

        private readonly string CurseModifier = "modifier_winter_wyvern_winters_curse";

        public TaskHandler HealHandler { get; private set; }

        public WinterWyvernCombo(WinterWyvern hero)
            : base(hero)
        {
            this.WinterWyvern = hero;
            this.HealHandler = UpdateManager.Run(HealOnUpdate);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, 1200);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible || this.CurrentTarget.HasModifier(CurseModifier))
            {
                this.WinterWyvern.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            try
            {
                if ((this.WinterWyvern.BlinkDagger != null) &&
                    (this.WinterWyvern.BlinkDagger.Item.IsValid) && Owner.Distance2D(this.CurrentTarget) <= 1200 + 350 &&
                    !(Owner.Distance2D(this.CurrentTarget) <= 400) &&
                    this.WinterWyvern.BlinkDagger.CanBeCasted && this.WinterWyvern.UseBlink)
                {
                    var l = (this.Owner.Distance2D(this.CurrentTarget) - 350) / 350;
                    var posA = this.Owner.Position;
                    var posB = this.CurrentTarget.Position;
                    var x = (posA.X + (l * posB.X)) / (1 + l);
                    var y = (posA.Y + (l * posB.Y)) / (1 + l);
                    var position = new Vector3((int)x, (int)y, posA.Z);

                    this.WinterWyvern.BlinkDagger.UseAbility(position);
                    await Task.Delay(this.WinterWyvern.BlinkDagger.GetCastDelay(position), token);
                }

                var linkens = this.CurrentTarget.IsLinkensProtected();
                await BreakLinken(token);

                var ulti = this.WinterWyvern.WintersCurse;
                if (ulti.CanBeCasted() &&
                    this.WinterWyvern.HeroestoUlti.Value.IsEnabled(CurrentTarget.Name) &&
                    this.WinterWyvern.NearHeroesCount(this.CurrentTarget) >= this.WinterWyvern.HeroCountToUlti.Value &&
                    this.Owner.Distance2D(this.CurrentTarget) <= this.WinterWyvern.WintersCurse.CastRange &&
                    !this.CurrentTarget.IsStunned())
                {
                    ulti.UseAbility(this.CurrentTarget);
                    await Task.Delay((int)ulti.GetCastDelay(this.Owner, this.CurrentTarget) + 200, token);
                }

                var q = this.WinterWyvern.ArcticBurn;
                if (this.Owner.AghanimState())
                {
                    if (!q.IsToggled && q.CanBeCasted() &&
                        this.Owner.Distance2D(this.CurrentTarget) <=
                        AbilityExtensions.GetAbilitySpecialData(q, "attack_range_bonus") + this.Owner.AttackRange)
                    {
                        q.ToggleAbility();
                        await Task.Delay(50, token);
                    }
                    await Task.Delay(125, token);
                }
                else if (!this.Owner.AghanimState() && q.CanBeCasted() && this.Owner.Distance2D(this.CurrentTarget) <= 
                    AbilityExtensions.GetAbilitySpecialData(q, "attack_range_bonus") + this.Owner.AttackRange)
                {
                    q.UseAbility();
                    await Task.Delay(50, token);
                }

                var blast = this.WinterWyvern.SplinterBlast;
                var target = this.WinterWyvern.SplinterBlastUnit(this.CurrentTarget);
                var modifier = this.CurrentTarget.HasModifier("modifier_winter_wyvern_winters_curse_aura") ||
                               this.CurrentTarget.HasModifier("modifier_winter_wyvern_winters_curse")
                    ? this.CurrentTarget.Modifiers.FirstOrDefault(x =>
                        x.Name.Contains("modifier_winter_wyvern_winters_curse")) : null;

                if (modifier != null && target != null)
                {
                    if (blast.CanBeCasted() && !this.CurrentTarget.IsMagicImmune() && !target.IsMagicImmune() &&
                        this.Owner.Distance2D(target) <= blast.CastRange && 
                        modifier.RemainingTime <= this.WinterWyvern.GetBlastHitTime(this.CurrentTarget) + (target.Distance2D(this.CurrentTarget) / 650))
                    {
                        blast.UseAbility(target);
                        await Task.Delay((int)blast.GetCastDelay(this.Owner, target), token);
                    }
                }

                else if (modifier == null && target != null && 
                         (this.WinterWyvern.NearHeroesCount(this.CurrentTarget) < this.WinterWyvern.HeroCountToUlti.Value || !this.WinterWyvern.WintersCurse.CanBeCasted()))
                {
                    if (blast.CanBeCasted() && !this.CurrentTarget.IsMagicImmune() && !target.IsMagicImmune() &&
                        this.Owner.Distance2D(target) <= blast.CastRange)
                    {
                        blast.UseAbility(target);
                        await Task.Delay((int)blast.GetCastDelay(this.Owner, target), token);
                    }
                }
                
                var abyssal = this.WinterWyvern.AbyssalBlade;
                if (abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
                    this.WinterWyvern.AbyssalBladeHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    abyssal.UseAbility(CurrentTarget);
                    await Task.Delay(abyssal.GetCastDelay(), token);
                }

                var manta = this.WinterWyvern.Manta;
                if (manta != null && manta.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 &&
                    this.WinterWyvern.MantaHeroes.Value.IsEnabled(CurrentTarget.Name))
                {
                    manta.UseAbility();
                    await Task.Delay(manta.GetCastDelay(), token);
                }

                var nullifier = this.WinterWyvern.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.WinterWyvern.NullifierHeroes.Value.IsEnabled(CurrentTarget.Name))
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

        private async Task HealOnUpdate(CancellationToken token)
        {
            if (Game.IsPaused)
            {
                await Task.Delay(250, token);
                return;
            }

            if ((this.WinterWyvern.HealOnlyInCombo && !this.WinterWyvern.Config.General.ComboKey.Value.Active))
            {
                await Task.Delay(250, token);
                return;
            }

            var allies =
                EntityManager<Hero>.Entities.Where(
                        x =>
                            x.IsValid && x.IsAlive && !x.IsIllusion && x.Team == this.Owner.Team &&
                            (((float)x.Health / x.MaximumHealth) * 100f) <= this.WinterWyvern.HealThreshold.Value.Value &&
                            x.Distance2D(this.Owner) <= 1100 &&
                            this.WinterWyvern.HeroestoHeal.Value.IsEnabled(x.Name))
                    .ToList();

            if (allies.Any() && this.WinterWyvern.ColdEmbrace.CanBeCasted())
            {
                foreach (var ally in allies)
                {
                    this.WinterWyvern.ColdEmbrace.UseAbility(ally);
                    await Task.Delay((int)this.WinterWyvern.ColdEmbrace.GetCastDelay(this.Owner, ally), token);
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
                        breakerChanger = this.WinterWyvern.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.WinterWyvern.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.WinterWyvern.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.WinterWyvern.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.WinterWyvern.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.WinterWyvern.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.WinterWyvern.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.WinterWyvern.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.WinterWyvern.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.WinterWyvern.DiffusalBlade;
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