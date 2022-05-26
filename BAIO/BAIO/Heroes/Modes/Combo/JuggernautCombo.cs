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
using log4net;
using PlaySharp.Toolkit.Logging;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using UnitExtensions = Ensage.Common.Extensions.UnitExtensions;
using Ensage.SDK.Handlers;
using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

namespace BAIO.Heroes.Modes.Combo
{
    internal class JuggernautCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Juggernaut Juggernaut;

        public TaskHandler IllusionHandler { get; private set; }

        public JuggernautCombo(Juggernaut hero)
            : base(hero)
        {
            Juggernaut = hero;
            this.IllusionHandler = UpdateManager.Run(OnUpdate);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await ShouldExecute(token))
            {
                return;
            }

            if (Owner.HasModifier(Juggernaut.Omnislash.ModifierName) && this.CurrentTarget.IsEthereal())
            {
                var nullifier = Juggernaut.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                {
                    nullifier.UseAbility(this.CurrentTarget);
                    await Task.Delay(nullifier.GetCastDelay(), token);
                }
                await Task.Delay(125, token);
            }

            var blink = Juggernaut.BlinkDagger;
            if (blink != null)
            {
                MaxTargetRange = blink.CastRange * 1.5f;
            }

            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible)
            {
                Juggernaut.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                OrbwalkToTarget();
                return;
            }

            var isInvis = this.Owner.IsInvisible();

            var sBlade = this.Juggernaut.ShadowBlade;
            var sEdge = this.Juggernaut.SilverEdge;

            if (!isInvis && sBlade?.CanBeCasted == true || sEdge?.CanBeCasted == true
                && this.Owner.Distance2D(this.CurrentTarget) < 2000 &&
                this.Juggernaut.InvisHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                sBlade?.UseAbility();
                sEdge?.UseAbility();
            }


            if (isInvis && this.CurrentTarget != null && this.Owner.CanAttack() &&
                this.CurrentTarget.Distance2D(this.Owner) <= this.Owner.AttackRange)
            {
                this.Owner.Attack(this.CurrentTarget);
                await Task.Delay((int) (UnitExtensions.AttackBackswing(this.Owner) + Game.Ping), token);
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            var omni = Juggernaut.Omnislash;
            var swift = Juggernaut.SwiftSlash;
            var healthPercent = Owner.HealthPercent();
            var targetDistance = Owner.Distance2D(this.CurrentTarget);
            var attackRange = Owner.AttackRange(this.CurrentTarget);
            if (omni != null && !isInvis && !this.CurrentTarget.IsIllusion && omni.CanBeCasted &&
                omni.CanHit(this.CurrentTarget))
            {
                var useOmni = healthPercent < 0.15f;
                if (!useOmni)
                    if (targetDistance > attackRange * 1.4f &&
                        Owner.MovementSpeed < this.CurrentTarget.MovementSpeed * 1.2f
                        || this.CurrentTarget.Health > omni.GetTickDamage(this.CurrentTarget) && this.Juggernaut.OmniHeroes.Value.IsEnabled(this.CurrentTarget.Name))
                    {
                        var unitsClose = EntityManager<Unit>.Entities.Where(
                                x => x.IsVisible
                                     && x.IsAlive
                                     && x != this.CurrentTarget
                                     && Owner.IsEnemy(x)
                                     && (x.IsIllusion || !(x is Hero))
                                     && !(x is Building)
                                     && x.IsRealUnit()
                                     && x.Distance2D(this.CurrentTarget) <= omni.Radius)
                            .ToList();

                        var abilityLevel = omni.Ability.Level - 1;

                        if (unitsClose.Count > 0 && unitsClose.Count <= abilityLevel)
                        {
                            var close = unitsClose;
                            foreach (var unit in close)
                            {
                                var close1 = unitsClose;
                                var unitsInRadius = EntityManager<Unit>.Entities.Where(
                                    x => !close1.Contains(x)
                                         && x.IsVisible
                                         && x.IsAlive
                                         && x != this.CurrentTarget
                                         && Owner.IsEnemy(x)
                                         && (x.IsIllusion || !(x is Hero))
                                         && !(x is Building)
                                         && x.IsRealUnit()
                                         && x.Distance2D(unit) < omni.Radius);
                                unitsClose = unitsClose.Concat(unitsInRadius).ToList();

                                if (unitsClose.Count > abilityLevel)
                                    break;
                            }
                        }

                        useOmni = unitsClose.Count <= abilityLevel;
                    }

                if (useOmni)
                {
                    Unit omniTarget;
                    if (this.CurrentTarget.IsReflectingAbilities())
                        omniTarget = EntityManager<Unit>
                            .Entities.Where(
                                x => x.IsVisible
                                     && x.IsAlive
                                     && Owner.IsEnemy(x)
                                     && !(x is Building)
                                     && x.IsRealUnit()
                                     && !x.IsReflectingAbilities()
                                     && x.Distance2D(Owner) < omni.CastRange)
                            .OrderBy(x => x.Distance2D(this.CurrentTarget))
                            .FirstOrDefault();
                    else
                        omniTarget = this.CurrentTarget;

                    if (omniTarget != null)
                    {
                        omni.UseAbility(omniTarget);
                        await Task.Delay(omni.GetCastDelay(omniTarget), token);
                    }
                }
            }

            if (swift != null && !isInvis && !this.CurrentTarget.IsIllusion && AbilityExtensions.CanBeCasted(swift) &&
                swift.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsReflectingAbilities())
            {
                if (swift.UseAbility(this.CurrentTarget))
                {
                    await Task.Delay(omni.GetCastDelay(this.CurrentTarget), token);
                }
            }

            var bladeFury = Juggernaut.BladeFury;
            if (blink != null && !this.CurrentTarget.IsIllusion && blink.CanBeCasted &&
                blink.CanHit(this.CurrentTarget) && this.Juggernaut.BladeFuryOnCombo)
            {
                var useBlink = omni != null && omni.CanBeCasted && !omni.CanHit(this.CurrentTarget);
                if (!useBlink)
                    if (targetDistance > 600)
                    {
                        var enemyCount = EntityManager<Hero>.Entities.Count(
                            x => x.IsAlive
                                 && x.IsVisible
                                 && x != this.CurrentTarget
                                 && Owner.IsEnemy(x)
                                 && !x.IsIllusion
                                 && x.Distance2D(this.CurrentTarget) < 800);
                        useBlink = enemyCount <= 1 || bladeFury != null && bladeFury.CanBeCasted;
                    }

                if (useBlink)
                {
                    var blinkPos = this.CurrentTarget.IsMoving
                        ? this.CurrentTarget.InFront(75)
                        : this.CurrentTarget.Position;
                    blink.UseAbility(blinkPos);
                    await Task.Delay(blink.GetCastDelay(blinkPos), token);
                }
            }

            var mom = this.Juggernaut.Mom;
            var cantCast = this.Owner.HasModifier(this.Juggernaut.Omnislash.ModifierName);

            if (mom != null && mom.CanBeCasted && cantCast)
            {
                mom.UseAbility();
                await Task.Delay(mom.GetCastDelay(), token);
            }

            var solarCrest = this.Juggernaut.SolarCrest;
            if (solarCrest != null && solarCrest.CanBeCasted && solarCrest.CanHit(this.CurrentTarget))
            {
                solarCrest.UseAbility(this.CurrentTarget);
                await Task.Delay(solarCrest.GetCastDelay(), token);
            }

            var bloodthorn = this.Juggernaut.BloodThorn;
            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens)
            {
                bloodthorn.UseAbility(this.CurrentTarget);
                await Task.Delay(bloodthorn.GetCastDelay(), token);
            }

            var orchid = this.Juggernaut.Orchid;
            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens)
            {
                orchid.UseAbility(this.CurrentTarget);
                await Task.Delay(orchid.GetCastDelay(), token);
            }

            var healingWard = Juggernaut.HealingWardAbility;
            if (healingWard != null && !isInvis && healingWard.CanBeCasted)
            {
                var recentDmgPercent = (float) Juggernaut.Owner.RecentDamage / Juggernaut.Owner.MaximumHealth;

                if (healthPercent < 0.35f || recentDmgPercent > 0.2)
                {
                    healingWard.UseAbility(Owner.Position);
                    await Task.Delay(healingWard.GetCastDelay(Owner.Position), token);
                }
            }

            if (!this.CurrentTarget.IsStunned() && !linkens && !this.CurrentTarget.IsIllusion &&
                Juggernaut.AbyssalBladeHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                var abysal = Juggernaut.AbyssalBlade;
                if (abysal != null && abysal.CanBeCasted && abysal.CanHit(this.CurrentTarget))
                {
                    abysal.UseAbility(this.CurrentTarget);
                    await Task.Delay(abysal.GetCastDelay(this.CurrentTarget), token);
                }
            }

            var manta = Juggernaut.Manta;
            if (manta != null && manta.CanBeCasted && !isInvis &&
                (targetDistance < attackRange * 1.3f && Juggernaut.MantaHeroes.Value.IsEnabled(this.CurrentTarget.Name)
                 || Owner.IsSilenced() || Owner.IsRooted()))
            {
                var isSilenced = Owner.IsSilenced();
                manta.UseAbility();
                await Task.Delay(manta.GetCastDelay(), token);
                if (isSilenced)
                    return;
            }

            var mjollnir = Juggernaut.Mjollnir;
            if (mjollnir != null && mjollnir.CanBeCasted && !isInvis && mjollnir.CanHit(this.CurrentTarget))
            {
                mjollnir.UseAbility(Owner);
                await Task.Delay(mjollnir.GetCastDelay(), token);
            }

            var diffusal = Juggernaut.DiffusalBlade;
            if (diffusal != null && !linkens && !this.CurrentTarget.IsIllusion && diffusal.CanBeCasted && !isInvis &&
                diffusal.CanHit(this.CurrentTarget) &&
                Juggernaut.DiffusalHeroes.Value.IsEnabled(this.CurrentTarget.Name))
            {
                diffusal.UseAbility(CurrentTarget);
                await Task.Delay(diffusal.GetCastDelay(), token);
            }

            var nullfier = Juggernaut.Nullifier;
            if (nullfier != null && !linkens && !this.CurrentTarget.IsIllusion && nullfier.CanBeCasted && !isInvis &&
                nullfier.CanHit(this.CurrentTarget))
            {
                nullfier.UseAbility(this.CurrentTarget);
                await Task.Delay(nullfier.GetCastDelay(this.CurrentTarget), token);
            }

            if (bladeFury != null && bladeFury.CanBeCasted && !isInvis && bladeFury.CanHit(CurrentTarget) && this.Juggernaut.BladeFuryOnCombo)
            {
                var enemyCount = EntityManager<Hero>.Entities.Count(
                    x => x.IsAlive
                         && x.IsVisible
                         && x != this.CurrentTarget
                         && this.Owner.IsEnemy(x)
                         && !x.IsIllusion
                         && x.Distance2D(this.CurrentTarget) < 800);

                if (enemyCount > 1
                    || !this.CurrentTarget.IsIllusion
                    && bladeFury.GetTickDamage(this.CurrentTarget) >
                    Owner.GetAttackDamage(this.CurrentTarget) * bladeFury.TickRate
                    && bladeFury.GetTotalDamage(this.CurrentTarget) >= 0.5f * this.CurrentTarget.Health)
                {
                    bladeFury.UseAbility();
                    await Task.Delay(bladeFury.GetCastDelay(), token);
                }
            }

            if (this.Juggernaut.BladeFuryOnlyChase && this.Owner.HasModifier(bladeFury.ModifierName))
            {
                this.Owner.Move(this.CurrentTarget.InFront(20));
            }
            else
            {
                this.OrbwalkToTarget();
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Juggernaut.Config.General.ComboKey.Value.Active)
            {
                await Task.Delay(250, token);
                return;
            }

            var illusions =
                EntityManager<Unit>.Entities.Where(
                        x => x.IsValid && x.IsAlive && x.IsIllusion && x.Team == this.Owner.Team && x.IsControllable)
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
                        breakerChanger = this.Juggernaut.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Juggernaut.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Juggernaut.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Juggernaut.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Juggernaut.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Juggernaut.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Juggernaut.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(
                                nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget),
                                token);
                            return;
                        }

                        var atos = this.Juggernaut.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(
                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Juggernaut.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Juggernaut.DiffusalBlade;
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