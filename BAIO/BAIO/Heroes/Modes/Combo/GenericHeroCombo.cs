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
using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;

namespace BAIO.Heroes.Modes.Combo
{
    internal class GenericHeroCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly GenericHero GenericHero;

        public TaskHandler EulComboHandler { get; private set; }

        public GenericHeroCombo(GenericHero hero)
            : base(hero)
        {
            this.GenericHero = hero;
            this.EulComboHandler = UpdateManager.Run(EulCombo);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = this.GenericHero.MaximumTargetDistance.Value;

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.GenericHero.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            if (UnitExtensions.HasModifier(this.Owner, "modifier_templar_assassin_meld"))
            {
                this.Owner.Attack(this.CurrentTarget);
            }

            try
            {
                if (!this.Owner.IsChanneling() && !UnitExtensions.HasModifier(this.Owner, "modifier_templar_assassin_meld"))
                {
                    this.OrbwalkToTarget();
                }

                if (this.GenericHero.PrioritizeBlink /*&& !this.Owner.IsChanneling()*/)
                {
                    if (!await this.GenericHero.MoveOrBlinkToEnemy(this.CurrentTarget, token))
                    {
                        return;
                    }
                }

                var linkens = this.CurrentTarget.IsLinkensProtected();
                if (linkens)
                {
                    await BreakLinken(token);

                }

                if (!this.GenericHero.PrioritizeAbilities)
                {
                    await GenericHero.UseGenericItems(this.CurrentTarget, token);

                    await GenericHero.DisableEnemy(this.CurrentTarget, token);

                    await GenericHero.UseGenericAbilities(this.CurrentTarget, token, 0.5f);

                    await Task.Delay(100, token);
                }
                else
                {
                    await GenericHero.UseGenericAbilities(this.CurrentTarget, token, 0.5f);

                    await Task.Delay(100, token);

                    await GenericHero.UseGenericItems(this.CurrentTarget, token);

                    await GenericHero.DisableEnemy(this.CurrentTarget, token);

                    await Task.Delay(100, token);

                }

                if (!await this.GenericHero.MoveOrBlinkToEnemy(this.CurrentTarget, token))
                {
                    return;
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
        }

        // todo
        public async Task EulCombo(CancellationToken token)
        {
            if (Game.IsPaused || this.Owner.UnitState.HasFlag(UnitState.Silenced) || this.CurrentTarget == null)
            {
                await Task.Delay(250, token);
                return;
            }

            var dict = new[]
            {
                "modifier_obsidian_destroyer_astral_imprisonment_prison",
                "modifier_eul_cyclone",
                "modifier_shadow_demon_disruption"
            };
            try
            {
                if (this.CurrentTarget.HasModifiers(dict, false))
                {
                    foreach (var abilityPair in this.GenericHero.EulAbilities)
                    {
                        var abilityDelay = abilityPair.Value;
                        var ability = this.Context.Owner.GetAbilityById(abilityPair.Key);
                        if (ability != null && ability.CanBeCasted())
                        {
                            foreach (var modifier in this.CurrentTarget.Modifiers)
                            {
                                if (dict.Contains(modifier.Name))
                                {
                                    var ping = Game.Ping / 1000f;
                                    var remainingTime = modifier.RemainingTime - ping - (abilityDelay);
                                    remainingTime *= 1000f;
                                    await Task.Delay(Math.Max((int)remainingTime, 0), token);

                                    if (ability.CanBeCasted())
                                    {
                                        ability.UseAbility(this.CurrentTarget.Position);
                                        await Task.Delay(GenericHero.GetAbilityCastDelay(ability, Owner, this.CurrentTarget), token);
                                    }
                                }
                            }
                        }
                    }

                    await Task.Delay(500, token);
                }
                else if (this.GenericHero.DisableAbilityToggler.Value.IsEnabled("item_cyclone") && !this.CurrentTarget.UnitState.HasFlag(UnitState.Stunned))
                {
                    if (this.GenericHero.Euls != null && this.GenericHero.Euls.CanBeCasted)
                    {
                        this.GenericHero.Euls.UseAbility(this.CurrentTarget);
                        await Task.Delay(this.GenericHero.Euls.GetCastDelay() + 200, token);
                    }
                }
            }
            catch (Exception)
            {
                // ignore
            }

            await Task.Delay(100, token);
        }

        protected async Task BreakLinken(CancellationToken token)
        {
            if (!this.GenericHero.Config.General.Enabled)
            {
                return;
            }

            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<string, uint>> breakerChanger = new List<KeyValuePair<string, uint>>();

                    if (this.CurrentTarget.IsLinkensProtected())
                    {
                        breakerChanger = this.GenericHero.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.GenericHero.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.GenericHero.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.GenericHero.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.GenericHero.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.GenericHero.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.GenericHero.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.GenericHero.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.GenericHero.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.GenericHero.DiffusalBlade;
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