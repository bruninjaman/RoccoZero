using System;
using System.Collections.Generic;
using Ensage.Common.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Prediction;
using Ensage.SDK.Prediction.Collision;
using Ensage.SDK.Renderer.Particle;
using SharpDX;
using SharpDX.DXGI;

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

    internal class TuskCombo : ComboMode
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Vector3 BlinkPosition;
        private readonly Tusk Tusk;
        private List<Hero> teammates;
        public TaskHandler ComboHandler;
        private Vector3 pos;
        private Vector3 shardPredPos;

        public TuskCombo(Tusk hero)
            : base(hero)
        {
            this.Tusk = hero;
            this.ComboHandler = UpdateManager.Run(this.KickCombo);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            var blink = this.Tusk.BlinkDagger;
            if (blink != null)
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange * 1.1f);
            }
            var snowball = this.Tusk.Snowball;
            if (snowball != null && snowball.CanBeCasted())
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, snowball.CastRange * 1.1f);
            }

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Tusk.Context.Orbwalker.Active.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }
            var ulti = this.Tusk.WalrusPunch;
            var shards = this.Tusk.IceShards;
            var lSnowball = this.Tusk.LaunchSnowball;
            // var kick = this.Tusk.WalrusKick;
            var tagTeam = this.Tusk.TagTeam;

            var blinkReady = blink != null && blink.CanBeCasted;
            var distance = this.Owner.Distance2D(this.CurrentTarget);

            if (this.Owner.IsInvisible())
            {
                if (ulti.CanBeCasted() && this.Owner.CanAttack())
                {
                    ulti.UseAbility(this.CurrentTarget);
                    await Task.Delay((int) ulti.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                }
                else
                {
                    this.Owner.Attack(CurrentTarget);
                    await Task.Delay(200, token);
                }
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            try
            {
                if (blinkReady && this.Owner.Distance2D(this.CurrentTarget) > 600)
                {
                    this.BlinkPosition = this.CurrentTarget.NetworkPosition.Extend(this.Owner.NetworkPosition,
                        Math.Max(100, distance - blink.CastRange));

                    blink.UseAbility(BlinkPosition);


                    var solar = this.Tusk.SolarCrest;
                    if (!this.Tusk.InSnowball && solar != null && solar.CanBeCasted && solar.CanHit(CurrentTarget)
                        && this.Tusk.MedallionCrestHeroes.Value.IsEnabled(this.CurrentTarget.Name))
                    {
                        solar.UseAbility(this.CurrentTarget);
                        await Task.Delay(50, token);
                    }
                    var medal = this.Tusk.Medallion;
                    if (!this.Tusk.InSnowball && medal != null && medal.CanBeCasted && medal.CanHit(CurrentTarget)
                        && this.Tusk.MedallionCrestHeroes.Value.IsEnabled(this.CurrentTarget.Name))
                    {
                        medal.UseAbility(this.CurrentTarget);
                        await Task.Delay(50, token);
                    }

                    var nullifier = this.Tusk.Nullifier;
                    if (!this.Tusk.InSnowball && nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget))
                    {
                        nullifier.UseAbility(this.CurrentTarget);
                        await Task.Delay(50, token);
                    }

                    var bloodthorn2 = this.Tusk.BloodThorn;
                    if (!this.Tusk.InSnowball && bloodthorn2 != null && bloodthorn2.CanBeCasted && bloodthorn2.CanHit(CurrentTarget))
                    {
                        bloodthorn2.UseAbility(this.CurrentTarget);
                        await Task.Delay(50, token);
                    }

                    if (ulti.CanBeCasted() && this.Owner.CanAttack())
                    {
                        ulti.UseAbility(this.CurrentTarget);
                        await Task.Delay((int) ulti.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                    }
                    else
                    {
                        await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
                    }
                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
                }
            }
            catch (TaskCanceledException)
            {
                // 
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            var solar2 = this.Tusk.SolarCrest;
            if (!this.Tusk.InSnowball && solar2 != null && solar2.CanBeCasted && solar2.CanHit(CurrentTarget)
                && this.Tusk.MedallionCrestHeroes.Value.IsEnabled(this.CurrentTarget.Name))
            {
                solar2.UseAbility(this.CurrentTarget);
                await Task.Delay(50, token);
            }
            var medal2 = this.Tusk.Medallion;
            if (!this.Tusk.InSnowball && medal2 != null && medal2.CanBeCasted && medal2.CanHit(CurrentTarget)
                && this.Tusk.MedallionCrestHeroes.Value.IsEnabled(this.CurrentTarget.Name))
            {
                medal2.UseAbility(this.CurrentTarget);
                await Task.Delay(50, token);
            }
            var nullifier2 = this.Tusk.Nullifier;
            if (!this.Tusk.InSnowball && nullifier2 != null && nullifier2.CanBeCasted && nullifier2.CanHit(CurrentTarget))
            {
                nullifier2.UseAbility(this.CurrentTarget);
                await Task.Delay(50, token);
            }

            var bloodthorn3 = this.Tusk.BloodThorn;
            if (!this.Tusk.InSnowball && bloodthorn3 != null && bloodthorn3.CanBeCasted && bloodthorn3.CanHit(CurrentTarget))
            {
                bloodthorn3.UseAbility(this.CurrentTarget);
                await Task.Delay(50, token);
            }
            if (ulti.CanBeCasted() && this.Owner.CanAttack())
            {
                ulti.UseAbility(this.CurrentTarget);
                await Task.Delay((int) ulti.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
            }


            if (!blinkReady && snowball.CanBeCasted() && snowball.CanHit(this.CurrentTarget) && distance >= 500)
            {
                if (snowball.CanBeCasted() && snowball.CanHit(this.CurrentTarget) && !linkens)
                {
                    snowball.UseAbility(this.CurrentTarget);
                    await Task.Delay(500, token);
                }

                if (this.Tusk.InSnowball)
                {
                    teammates =
                        EntityManager<Hero>.Entities.Where(
                                x =>
                                    x.IsValid && !x.IsChanneling() && x.Team == this.Owner.Team && x != this.Owner &&
                                    x.Distance2D(this.Owner) <= 400 &&
                                    !x.HasModifier("modifier_tusk_snowball_movement_friendly"))
                            .OrderBy(x => x.Distance2D(this.Owner)).ToList();

                    if (teammates.Any())
                    {
                        foreach (var teammate in teammates)
                        {
                            this.Owner.Attack(teammate);
                        }
                        if (this.Tusk.InSnowball && lSnowball != null && lSnowball.CanBeCasted())
                        {
                            lSnowball.UseAbility();
                            //await Task.Delay((int)lSnowball.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                        }
                    }
                    else if (!teammates.Any())
                    {
                        if (this.Tusk.InSnowball && lSnowball != null && lSnowball.CanBeCasted())
                        {
                            lSnowball.UseAbility();
                            //await Task.Delay((int)lSnowball.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                        }
                    }
                }
            }
            try
            {
                if (shards.CanBeCasted() && shards.CanHit(this.CurrentTarget))
                {
                    if (this.Tusk.InSnowball && distance > 750)
                    {
                        return;
                    }

                    if (ulti.CanBeCasted() && distance <= 300)
                    {
                        return;
                    }

                    var input = new PredictionInput(this.Owner,
                        this.CurrentTarget,
                        (int) shards.GetCastDelay(this.Owner, this.CurrentTarget) / 1000f,
                        1200,
                        2000,
                        100,
                        PredictionSkillshotType.SkillshotCircle)
                    {
                        CollisionTypes = CollisionTypes.None
                    };

                    var output = this.Context.Prediction.GetPrediction(input);
                    var asdf = this.Owner.InFront(200 + this.Owner.Distance2D(this.CurrentTarget));
                    var arrivalTime = this.Owner.Distance2D(this.CurrentTarget) / 2000;
                    var arrivalTimeMs = arrivalTime * this.CurrentTarget.MovementSpeed;
                    shardPredPos = output.CastPosition.Extend(this.Owner.NetworkPosition, -arrivalTimeMs - 150);
                    if (this.CurrentTarget.HasModifier("modifier_tusk_walrus_punch_air_time"))
                    {
                        shards.UseAbility(this.CurrentTarget.NetworkPosition.Extend(asdf, 200));
                        await Task.Delay((int) shards.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                    }

                    if (output.HitChance >= HitChance.Medium)
                    {
                        shards.UseAbility(shardPredPos);
                        await Task.Delay((int) shards.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // no
            }
            catch (Exception e)
            {
                Log.Error(e);
            }


            if (distance <= 300 && tagTeam.CanBeCasted())
            {
                tagTeam.UseAbility();
                await Task.Delay((int)tagTeam.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
            }

            var vessel = this.Tusk.Vessel;
            var urn = this.Tusk.Urn;
            if ((urn?.CanBeCasted == true && this.Tusk.UrnHeroes.Value.IsEnabled(CurrentTarget.Name) ||
                vessel?.CanBeCasted == true && this.Tusk.VesselHeroes.Value.IsEnabled(CurrentTarget.Name)) && !linkens
                && this.Owner.Distance2D(this.CurrentTarget) < 400)
            {
                urn?.UseAbility(this.CurrentTarget);
                vessel?.UseAbility(this.CurrentTarget);
            }

            var halberd = this.Tusk.HeavensHalberd;
            if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
                this.Tusk.HalberdHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                halberd.UseAbility(CurrentTarget);
                await Task.Delay(halberd.GetCastDelay(), token);
            }

            var atos = this.Tusk.RodOfAtos;
            if (atos != null && atos.CanBeCasted && atos.CanHit(CurrentTarget) && !linkens &&
                this.Tusk.AtosHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                atos.UseAbility(CurrentTarget);
                await Task.Delay(atos.GetCastDelay(), token);
            }

            var bloodthorn = this.Tusk.BloodThorn;
            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
                this.Tusk.OrchidBloodthornHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                bloodthorn.UseAbility(CurrentTarget);
                await Task.Delay(bloodthorn.GetCastDelay(), token);
            }

            var orchid = this.Tusk.Orchid;
            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
                this.Tusk.OrchidBloodthornHeroes.Value.IsEnabled(CurrentTarget.Name))
            {
                orchid.UseAbility(CurrentTarget);
                await Task.Delay(orchid.GetCastDelay(), token);
            }

            this.OrbwalkToTarget();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            ComboHandler.Cancel();
        }

        private async Task KickCombo(CancellationToken token)
        {
            var target = this.Context.TargetSelector.GetTargets().FirstOrDefault();
            if (Game.IsPaused || !this.Owner.IsAlive || !this.Tusk.WalrusKick.CanBeCasted() || target == null ||
                target.IsLinkensProtected() || target.HasModifier("modifier_item_lotus_orb_active"))
            {
                return;
            }

            if (this.Tusk.KickCombo.Value.Active)
            {
                try
                {
                    var kick = Tusk.WalrusKick;
                    var teammateys =
                        EntityManager<Hero>.Entities.Where(
                                x => x.IsValid && x.Team == this.Owner.Team && x != this.Owner && x.Distance2D(this.Owner) <= 1600 + this.Tusk.BlinkDagger.CastRange)
                            .OrderBy(x => x.Distance2D(this.Owner)).FirstOrDefault();

                    var fountain =
                        EntityManager<Unit>.Entities
                            .FirstOrDefault(x => x.IsValid && x.Team == this.Owner.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain);

                    if ((teammateys != null || fountain != null))
                    {

                        if (Tusk.InsecType == InsecType.TeamMate && teammateys != null)
                        {
                            pos = teammateys.NetworkPosition.Extend(target.NetworkPosition,
                                      teammateys.Distance2D(target) + kick.CastRange);
                        }
                        else if (Tusk.InsecType == InsecType.Fountain && fountain != null)
                        {
                            pos = fountain.NetworkPosition.Extend(target.NetworkPosition,
                                      fountain.Distance2D(target) + kick.CastRange + 100);
                        }

                        if (this.Tusk.BlinkDagger != null && this.Tusk.BlinkDagger.CanBeCasted && 
                            this.Owner.Distance2D(pos) <= this.Tusk.BlinkDagger.CastRange)
                        {
                            this.Tusk.BlinkDagger.UseAbility(pos);
                            await Task.Delay((int)this.Tusk.BlinkDagger.GetCastDelay(), token);
                        }

                        var ultBlyad = this.Tusk.WalrusPunch;
                        if (ultBlyad.CanBeCasted() && this.Owner.CanAttack())
                        {
                            ultBlyad.UseAbility(target);
                            await Task.Delay((int)ultBlyad.GetCastDelay(this.Owner, target, true) + 500, token);
                        }

                        if (kick.CanBeCasted() && kick.CanHit(target))
                        {
                            kick.UseAbility(target);
                            await Task.Delay(500 + (int)Game.Ping, token);
                        }

                        await Task.Delay(200, token);
                    }

                    var shard = this.Tusk.IceShards;
                    if (!kick.CanBeCasted() && shard.CanBeCasted())
                    {
                        var inFront = this.Owner.InFront(1700);
                        shard.UseAbility(inFront);
                        await Task.Delay((int)shard.GetCastDelay(this.Owner, this.CurrentTarget, true) + (int)Game.Ping, token);
                    }

                    var lSnowball = this.Tusk.LaunchSnowball;
                    var snowball = this.Tusk.Snowball;
                    if (!kick.CanBeCasted() && snowball.CanBeCasted() && snowball.CanHit(target))
                    {
                        snowball.UseAbility(target);
                        await Task.Delay((int)Game.Ping + 500, token);
                    }

                    if (lSnowball.CanBeCasted() && !snowball.CanBeCasted())
                    {
                        lSnowball.UseAbility();
                        await Task.Delay((int)Game.Ping + 1500, token);
                    }

                }
                catch (Exception e)
                {
                    Log.Error(e);
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
                        breakerChanger = this.Tusk.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
                                x => this.Tusk.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
                            .OrderByDescending(x => x.Value)
                            .ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Tusk.Euls;
                        if (euls != null
                            && euls.ToString() == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.UseAbility(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Tusk.ForceStaff;
                        if (force != null
                            && force.ToString() == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.UseAbility(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Tusk.Orchid;
                        if (orchid != null
                            && orchid.ToString() == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.UseAbility(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Tusk.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.ToString() == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.UseAbility(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Tusk.Nullifier;
                        if (nullifier != null
                            && nullifier.ToString() == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.UseAbility(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Tusk.RodOfAtos;
                        if (atos != null
                            && atos.ToString() == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.UseAbility(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Tusk.Sheepstick;
                        if (hex != null
                            && hex.ToString() == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.UseAbility(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Tusk.DiffusalBlade;
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