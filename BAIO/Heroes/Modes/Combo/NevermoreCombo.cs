//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Base;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common;
//using Ensage.Common.Extensions;
//using Ensage.Common.Threading;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Prediction;
//using log4net;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using AbilityExtensions = Ensage.SDK.Extensions.AbilityExtensions;
//using EnumerableExtensions = Ensage.SDK.Extensions.EnumerableExtensions;
//using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;

//namespace BAIO.Heroes.Modes.Combo
//{
//    internal class NevermoreCombo : ComboMode
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        public Nevermore Nevermore;
//        private float razeStartCastTime;
//        public float Damage;
//        private readonly IUpdateHandler razeUpdateHandler;
//        private TaskHandler ComboHandler;
//        private Unit EulTarget { get; set; }

//        public NevermoreCombo(Nevermore hero)
//            : base(hero)
//        {
//            this.Nevermore = hero;
//            Entity.OnBoolPropertyChange += this.OnShortRazeCast;
//            Entity.OnBoolPropertyChange += this.OnMediumRazeCast;
//            Entity.OnBoolPropertyChange += this.OnLongRazeCast;
//            this.razeUpdateHandler = UpdateManager.Subscribe(this.RazeHitCheck, 25, false);
//            this.ComboHandler = TaskHandler.Run(this.EulCombo);
//        }

//        protected override void OnDeactivate()
//        {
//            this.ComboHandler.Cancel();
//            UpdateManager.Unsubscribe(this.RazeHitCheck);

//            Entity.OnBoolPropertyChange -= this.OnShortRazeCast;
//            Entity.OnBoolPropertyChange -= this.OnMediumRazeCast;
//            Entity.OnBoolPropertyChange -= this.OnLongRazeCast;

//            base.OnDeactivate();
//        }

//        public override async Task ExecuteAsync(CancellationToken token)
//        {
//            if (!await this.ShouldExecute(token) || this.Owner.IsChanneling())
//            {
//                return;
//            }

//            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible || UnitExtensions.IsInvulnerable(this.CurrentTarget))
//            {
//                this.Nevermore.Context.Orbwalker.OrbwalkTo(null);
//                return;
//            }

//            if (this.CurrentTarget.IsIllusion)
//            {
//                this.OrbwalkToTarget();
//                return;
//            }

//            if (this.CurrentTarget != null && this.Owner.HasModifier("modifier_item_silver_edge_windwalk"))
//            {
//                this.Owner.Attack(this.CurrentTarget);
//                await Task.Delay(100, token);
//            }

//            if ((this.Nevermore.BlinkDagger != null) &&
//                (this.Nevermore.BlinkDagger.Item.IsValid) && Owner.Distance2D(this.CurrentTarget) <= 1200 + 350 &&
//                !(Owner.Distance2D(this.CurrentTarget) <= 400) &&
//                this.Nevermore.BlinkDagger.CanBeCasted && this.Nevermore.UseBlink)
//            {
//                var l = (this.Owner.Distance2D(this.CurrentTarget) - 350) / 350;
//                var posA = this.Owner.Position;
//                var posB = this.CurrentTarget.Position;
//                var x = (posA.X + (l * posB.X)) / (1 + l);
//                var y = (posA.Y + (l * posB.Y)) / (1 + l);
//                var position = new Vector3((int)x, (int)y, posA.Z);

//                this.Nevermore.BlinkDagger.Cast(position);
//                await Task.Delay(this.Nevermore.BlinkDagger.GetCastDelay(position), token);
//            }

//            var linkens = this.CurrentTarget.IsLinkensProtected();
//            await BreakLinken(token, this.CurrentTarget);

//            var veil = this.Nevermore.VeilOfDiscord;
//            if (veil != null && veil.CanBeCasted && veil.CanHit(CurrentTarget) &&
//                this.Nevermore.VeilOfDiscordHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                veil.Cast(CurrentTarget.Position);
//                await Task.Delay(veil.GetCastDelay(), token);
//            }

//            var sheep = this.Nevermore.Sheepstick;
//            if (sheep != null && sheep.CanBeCasted && !linkens &&
//                (this.Nevermore.HexHeroes[((Hero)CurrentTarget).HeroId]))
//            {
//                sheep.Cast(CurrentTarget);
//                await Task.Delay(sheep.GetCastDelay(), token);
//            }

//            var ethereal = this.Nevermore.EtherealBlade;
//            if (ethereal != null && ethereal.CanBeCasted && ethereal.CanHit(CurrentTarget) && !linkens &&
//                this.Nevermore.EtherealHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                ethereal.Cast(CurrentTarget);
//                await Task.Delay(ethereal.GetCastDelay(this.CurrentTarget), token);
//            }

//            var dagon = this.Nevermore.Dagon;
//            if ((ethereal == null || !ethereal.CanBeCasted) && dagon != null && dagon.CanBeCasted && !linkens &&
//                dagon.CanHit(this.CurrentTarget))
//            {
//                dagon.Cast(this.CurrentTarget);
//                await Task.Delay(dagon.GetCastDelay(), token);
//            }

//            var shivas = this.Nevermore.ShivasGuard;
//            if (!this.Owner.IsChanneling() && shivas != null && shivas.CanBeCasted && Owner.Distance2D(CurrentTarget) <= 700)
//            {
//                shivas.Cast();
//                await Task.Delay(shivas.GetCastDelay(), token);
//            }
//            var bloodthorn = this.Nevermore.BloodThorn;
//            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
//                this.Nevermore.OrchidBloodthornHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                bloodthorn.Cast(CurrentTarget);
//                await Task.Delay(bloodthorn.GetCastDelay(), token);
//            }
//            var orchid = this.Nevermore.Orchid;
//            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
//                this.Nevermore.OrchidBloodthornHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                orchid.Cast(CurrentTarget);
//                await Task.Delay(orchid.GetCastDelay(), token);
//            }
//            var atos = this.Nevermore.RodOfAtos;
//            if (atos != null && atos.CanBeCasted && atos.CanHit(CurrentTarget) && !linkens &&
//                this.Nevermore.AtosHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                atos.Cast(CurrentTarget);
//                await Task.Delay(atos.GetCastDelay(), token);
//            }
//            var halberd = this.Nevermore.HeavensHalberd;
//            if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
//                this.Nevermore.AtosHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                halberd.Cast(CurrentTarget);
//                await Task.Delay(halberd.GetCastDelay(), token);
//            }

//            try
//            {
//                // LogManager.Debug($"{Nevermore.Razes.ToString()}");
//                if (Nevermore.Razes != null)
//                {
//                    foreach (
//                        var raze in
//                        Nevermore.Razes.OrderBy(
//                            x => this.Owner.Distance2D(this.Owner.InFront(AbilityExtensions.GetAbilitySpecialData(x, "shadowraze_range")))))
//                    {
//                        if (CurrentTarget == null || UnitExtensions.IsMagicImmune(CurrentTarget) || raze == null ||
//                            !raze.CanBeCasted() || this.Owner.IsSilenced() ||
//                            !this.Nevermore.CanHit(raze, CurrentTarget, Nevermore.GetRazeDelay(CurrentTarget, raze)) || !this.CurrentTarget.IsAlive ||
//                            UnitExtensions.IsInvulnerable(this.CurrentTarget))
//                            continue;

//                        /*if (raze == this.Nevermore.RazeLong && this.Owner.Distance2D(this.CurrentTarget) >= 730)
//                        {
//                            return;
//                        }
//                        else
//                        {*/
//                        raze.Cast();
//                        await Task.Delay((int)raze.GetCastDelay(this.Owner, this.CurrentTarget, true) + 100, token);
//                        //}
//                    }
//                }
//            }
//            catch (TaskCanceledException)
//            {
//                // ignore
//            }
//            catch (Exception e)
//            {
//                LogManager.Debug(e);
//            }

//            if (!this.Nevermore.RazeShort.IsInAbilityPhase && !this.Nevermore.RazeMedium.IsInAbilityPhase &&
//                !this.Nevermore.RazeLong.IsInAbilityPhase)
//            {
//                this.OrbwalkToTarget();
//            }
//        }

//        private void OnShortRazeCast(Entity sender, BoolPropertyChangeEventArgs args)
//        {
//            if (args.NewValue == args.OldValue || sender != this.Nevermore.RazeShort ||
//                args.PropertyName != "m_bInAbilityPhase")
//            {
//                return;
//            }
//            if (args.NewValue)
//            {
//                this.razeStartCastTime = GameManager.RawGameTime;
//                this.razeUpdateHandler.IsEnabled = true;
//            }
//            else
//            {
//                this.razeUpdateHandler.IsEnabled = false;
//            }
//        }

//        private void OnMediumRazeCast(Entity sender, BoolPropertyChangeEventArgs args)
//        {
//            if (args.NewValue == args.OldValue || sender != this.Nevermore.RazeMedium ||
//                args.PropertyName != "m_bInAbilityPhase")
//            {
//                return;
//            }
//            if (args.NewValue)
//            {
//                this.razeStartCastTime = GameManager.RawGameTime;
//                this.razeUpdateHandler.IsEnabled = true;
//            }
//            else
//            {
//                this.razeUpdateHandler.IsEnabled = false;
//            }
//        }

//        private void OnLongRazeCast(Entity sender, BoolPropertyChangeEventArgs args)
//        {
//            if (args.NewValue == args.OldValue || sender != this.Nevermore.RazeLong ||
//                args.PropertyName != "m_bInAbilityPhase")
//            {
//                return;
//            }
//            if (args.NewValue)
//            {
//                this.razeStartCastTime = GameManager.RawGameTime;
//                this.razeUpdateHandler.IsEnabled = true;
//            }
//            else
//            {
//                this.razeUpdateHandler.IsEnabled = false;
//            }
//        }

//        private void RazeHitCheck()
//        {
//            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible || !Nevermore.PriRaze)
//            {
//                return;
//            }

//            if (this.Nevermore.RazeShort.IsInAbilityPhase &&
//                !this.Nevermore.CanHit(Nevermore.RazeShort, CurrentTarget,
//                    this.Nevermore.GetRazeDelay(CurrentTarget, Nevermore.RazeShort)) || (UnitExtensions.IsInvulnerable(this.CurrentTarget)))
//            {
//                this.Owner.Stop();
//                this.Owner.MoveToDirection(this.Nevermore.PredictedTargetPosition);
//                this.razeUpdateHandler.IsEnabled = false;
//                Task.Delay(200);
//            }

//            if (this.Nevermore.RazeMedium.IsInAbilityPhase &&
//                !this.Nevermore.CanHit(Nevermore.RazeMedium, CurrentTarget,
//                    this.Nevermore.GetRazeDelay(CurrentTarget, Nevermore.RazeMedium)) || (UnitExtensions.IsInvulnerable(this.CurrentTarget)))
//            {
//                this.Owner.Stop();
//                this.Owner.MoveToDirection(this.Nevermore.PredictedTargetPosition);
//                this.razeUpdateHandler.IsEnabled = false;
//                Task.Delay(200);
//            }

//            if (this.Nevermore.RazeLong.IsInAbilityPhase &&
//                !this.Nevermore.CanHit(Nevermore.RazeLong, CurrentTarget,
//                    this.Nevermore.GetRazeDelay(CurrentTarget, Nevermore.RazeLong)) && this.Owner.Distance2D(this.CurrentTarget) <= 730
//                     || (UnitExtensions.IsInvulnerable(this.CurrentTarget)))
//            {
//                this.Owner.Stop();
//                this.Owner.MoveToDirection(this.Nevermore.PredictedTargetPosition);
//                this.razeUpdateHandler.IsEnabled = false;
//                Task.Delay(200);
//            }
//        }

//        private async Task EulCombo(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Owner.IsAlive)
//            {
//                return;
//            }

//            if (this.Nevermore.UltiCombo.Value.Active)
//            {
//                if ((this.EulTarget == null) || !this.EulTarget.IsValid || !this.EulTarget.IsAlive)
//                {
//                    EulTarget = this.Context.TargetSelector.Active.GetTargets().FirstOrDefault();
//                }
//            }
//            else
//            {
//                EulTarget = null;
//            }


//            if (!this.Nevermore.Ulti.CanBeCasted() || EulTarget == null)
//            {
//                if (this.Nevermore.UltiCombo.Value.Active)
//                {
//                    this.Context.Orbwalker.OrbwalkTo(null);
//                    return;
//                }
//                //await Task.Delay(250, token);
//                return;
//            }

//            if (this.Nevermore.UltiCombo.Value.Active && EulTarget != null)
//            {
//                try
//                {
//                    if (this.Owner.Distance2D(EulTarget) >= 2000 && !this.Owner.IsChanneling() || EulTarget.HasModifier("modifier_item_lotus_orb_active"))
//                    {
//                        this.Context.Orbwalker.OrbwalkTo(null);
//                    }
//                    var ping = GameManager.Ping / 1000f;
//                    var eul = this.Nevermore.Euls;
//                    var hex = this.Nevermore.Sheepstick;
//                    var silverEdge = this.Nevermore.SilverEdge;
//                    var shadowBlade = this.Nevermore.ShadowBlade;
//                    var blink = this.Nevermore.BlinkDagger;
//                    var bkb = this.Nevermore.BlackKingBar;
//                    var veil = this.Nevermore.VeilOfDiscord;
//                    var shivas = this.Nevermore.ShivasGuard;
//                    var modifier = EulTarget.FindModifier("modifier_eul_cyclone");
//                    var targetCanEvade =
//                        EulTarget.Spellbook.Spells.Any(x => this.Nevermore.EvadeAbilities.Contains(x.Name) && x.CanBeCasted()) ||
//                        EulTarget.Inventory.Items.Any(x => this.Nevermore.EvadeItems.Contains(x.Name) && x.CanBeCasted());

//                    var linkens = EulTarget.IsLinkensProtected();
//                    await BreakLinken(token, EulTarget);

//                    if (eul != null && eul.CanBeCasted && !EulTarget.IsMagicImmune())
//                    {
//                        if (blink != null && blink.CanBeCasted && Owner.Distance2D(EulTarget) >= 300 && Owner.Distance2D(EulTarget) <= 1300)
//                        {
//                            blink.Cast(EulTarget.Position);
//                            await Task.Delay(blink.GetCastDelay() + 50, token);
//                        }

//                        if (veil != null && veil.CanBeCasted &&
//                            this.Nevermore.VeilOfDiscordHeroes.Value.IsEnabled(EulTarget.Name))
//                        {
//                            veil.Cast(EulTarget.Position);
//                            await Task.Delay(veil.GetCastDelay(), token);
//                        }

//                        if (hex != null && hex.CanBeCasted && !linkens &&
//                            (targetCanEvade || this.Nevermore.HexHeroes.Value.IsEnabled(EulTarget.Name)))
//                        {
//                            hex.Cast(EulTarget);
//                            await Task.Delay(hex.GetCastDelay(), token);
//                        }

//                        await BreakLinken(token, EulTarget);
//                        await Task.Delay(50, token);

//                        var prediction = (this.Owner.Distance2D(EulTarget) / this.Owner.MovementSpeed) + 1.6f;
//                        if (eul.CanBeCasted && !EulTarget.IsMagicImmune() && (hex == null || !hex.CanBeCasted || !targetCanEvade) && !linkens &&
//                            2.5f >= prediction)
//                        {
//                            if (eul.Cast(EulTarget))
//                            {
//                                await Task.Delay(eul.GetCastDelay() + ((int)GameManager.Ping / 1000) + 100, token);
//                            }
//                        }
//                        else
//                        {
//                            this.Context.Orbwalker.OrbwalkTo(null);
//                            return;
//                        }
//                    }

//                    else if (this.Nevermore.EullessUlti && (eul == null || eul.Item.Cooldown > 0) && modifier == null && this.Owner.Distance2D(EulTarget) <= blink?.CastRange && blink.CanBeCasted)
//                    {
//                        // LogManager.Debug($"going ulti combo w/o euls");

//                        if (blink.CanBeCasted && Owner.Distance2D(EulTarget) >= 300 && Owner.Distance2D(EulTarget) <= blink.CastRange)
//                        {
//                            blink.Cast(EulTarget.Position);
//                            await Task.Delay(blink.GetCastDelay() + 40, token);
//                        }

//                        if (hex != null && hex.CanBeCasted && hex.CanHit(EulTarget) && !linkens &&
//                            (targetCanEvade || this.Nevermore.HexHeroes.Value.IsEnabled(EulTarget.Name)))
//                        {
//                            hex.Cast(EulTarget);
//                            await Task.Delay(hex.GetCastDelay(), token);
//                        }

//                        if (veil != null && veil.CanBeCasted && veil.CanHit(EulTarget) &&
//                            this.Nevermore.VeilOfDiscordHeroes.Value.IsEnabled(EulTarget.Name))
//                        {
//                            veil.Cast(EulTarget.Position);
//                            await Task.Delay(veil.GetCastDelay(), token);
//                        }
//                        if (bkb != null && bkb.CanBeCasted && !this.Owner.IsInvisible() && this.Nevermore.BkbToggle.Value.Active)
//                        {
//                            bkb.Cast();
//                            await Task.Delay(bkb.GetCastDelay(), token);
//                        }

//                        if (shivas != null && shivas.CanBeCasted && Owner.Distance2D(EulTarget) <= blink.CastRange)
//                        {
//                            shivas.Cast();
//                            await Task.Delay(shivas.GetCastDelay(), token);
//                        }

//                        if (this.Nevermore.Ulti.CanBeCasted())
//                        {
//                            if (this.Nevermore.Ulti.Cast())
//                            {
//                                await Task.Delay(1000, token);
//                            }
//                        }

//                        if (modifier != null && this.Nevermore.Ulti.IsInAbilityPhase)
//                        {
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        this.Owner.Move(EulTarget.Position);
//                        await Task.Delay(100, token);
//                    }

//                    if (modifier == null)
//                    {
//                        return;
//                    }
//                    else
//                    {
//                        this.Owner.Move(EulTarget.Position);
//                        await Task.Delay(100, token);
//                    }

//                    if (bkb != null && bkb.CanBeCasted && !this.Owner.IsInvisible() && this.Nevermore.BkbToggle.Value.Active)
//                    {
//                        bkb.Cast();
//                        await Task.Delay(bkb.GetCastDelay(), token);
//                    }
//                    else if (silverEdge != null && silverEdge.CanBeCasted)
//                    {
//                        silverEdge.Cast();
//                        await Task.Delay(silverEdge.GetCastDelay(), token);
//                    }
//                    else if (shadowBlade != null && shadowBlade.CanBeCasted)
//                    {
//                        shadowBlade.Cast();
//                        await Task.Delay(shadowBlade.GetCastDelay(), token);
//                    }

//                    var remainingTime = modifier.RemainingTime - ping - 1.60f;
//                    remainingTime *= 1000f;
//                    await Task.Delay(Math.Max((int)remainingTime, 0), token);

//                    if (this.Nevermore.Ulti.CanBeCasted())
//                    {
//                        if (this.Nevermore.Ulti.Cast())
//                        {
//                            await Task.Delay(1000, token);
//                        }
//                    }

//                    if (!this.Owner.IsChanneling() && shivas != null && shivas.CanBeCasted && Owner.Distance2D(EulTarget) <= 500)
//                    {
//                        shivas.Cast();
//                        await Task.Delay(shivas.GetCastDelay(), token);
//                    }
//                    await Task.Delay(100, token);
//                }
//                catch (TaskCanceledException)
//                {
//                    // ignore
//                }
//                catch (Exception e)
//                {
//                    LogManager.Error(e);
//                }
//            }
//        }

//        protected async Task BreakLinken(CancellationToken token, Unit target)
//        {
//            if (target != null && target.IsValid)
//            {
//                try
//                {
//                    List<KeyValuePair<string, uint>> breakerChanger = new List<KeyValuePair<string, uint>>();

//                    if (target.IsLinkensProtected())
//                    {
//                        breakerChanger = this.Nevermore.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
//                                x => this.Nevermore.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
//                            .OrderByDescending(x => x.Value)
//                            .ToList();
//                    }

//                    foreach (var order in breakerChanger)
//                    {
//                        var euls = this.Nevermore.Euls;
//                        if (euls != null
//                            && euls.Item.Id == order.Key
//                            && euls.CanBeCasted && euls.CanHit(target))
//                        {
//                            euls.Cast(target);
//                            await Task.Delay(euls.GetCastDelay(target), token);
//                            return;
//                        }

//                        var force = this.Nevermore.ForceStaff;
//                        if (force != null
//                            && force.Item.Id == order.Key
//                            && force.CanBeCasted && force.CanHit(target))
//                        {
//                            force.Cast(target);
//                            await Task.Delay(force.GetCastDelay(target), token);
//                            return;
//                        }

//                        var orchid = this.Nevermore.Orchid;
//                        if (orchid != null
//                            && orchid.Item.Id == order.Key
//                            && orchid.CanBeCasted && orchid.CanHit(target))
//                        {
//                            orchid.Cast(target);
//                            await Task.Delay(orchid.GetCastDelay(target), token);
//                            return;
//                        }

//                        var bloodthorn = this.Nevermore.BloodThorn;
//                        if (bloodthorn != null
//                            && bloodthorn.Item.Id == order.Key
//                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(target))
//                        {
//                            bloodthorn.Cast(target);
//                            await Task.Delay(bloodthorn.GetCastDelay(target), token);
//                            return;
//                        }

//                        var nullifier = this.Nevermore.Nullifier;
//                        if (nullifier != null
//                            && nullifier.Item.Id == order.Key
//                            && nullifier.CanBeCasted && nullifier.CanHit(target))
//                        {
//                            nullifier.Cast(target);
//                            await Task.Delay(nullifier.GetCastDelay(target) + nullifier.GetHitTime(target), token);
//                            return;
//                        }

//                        var atos = this.Nevermore.RodOfAtos;
//                        if (atos != null
//                            && atos.Item.Id == order.Key
//                            && atos.CanBeCasted && atos.CanHit(target))
//                        {
//                            atos.Cast(target);
//                            await Task.Delay(atos.GetCastDelay(target) + atos.GetHitTime(target), token);
//                            return;
//                        }

//                        var hex = this.Nevermore.Sheepstick;
//                        if (hex != null
//                            && hex.Item.Id == order.Key
//                            && hex.CanBeCasted && hex.CanHit(target))
//                        {
//                            hex.Cast(target);
//                            await Task.Delay(hex.GetCastDelay(target), token);
//                            return;
//                        }

//                        var diff = this.Nevermore.DiffusalBlade;
//                        if (diff != null
//                            && diff.Item.Id == order.Key
//                            && diff.CanBeCasted && diff.CanHit(this.CurrentTarget))
//                        {
//                            diff.Cast(this.CurrentTarget);
//                            await Task.Delay(diff.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }
//                    }
//                }
//                catch (TaskCanceledException)
//                {
//                    // ignore
//                }
//                catch (Exception e)
//                {
//                    LogManager.Error("Linken break error: " + e);
//                }
//            }
//        }
//    }
//}