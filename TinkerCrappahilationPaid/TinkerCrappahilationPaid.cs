using System;
using System.Linq;
using System.Threading.Tasks;

using Divine;
using Divine.SDK.Helpers;
using Divine.SDK.Managers.Log;

using TinkerCrappahilationPaid.Abilities;

namespace TinkerCrappahilationPaid
{
    /*[ExportPlugin(
        mode: StartupMode.Auto,
        name: "TinkerCrappahilationPaid",
        version: "1.0.0.0",
        author: "JumpAttacker",
        description: "",
        units: new[] {HeroId.npc_dota_hero_tinker})]*/
    public sealed class TinkerCrappahilationPaid : Bootstrapper
    {
        public static readonly Log Log = LogManager.GetCurrentClassLogger();

        public AbilitiesInCombo AbilitiesInCombo { get; private set; }
        public KillSteal KillSteal { get; private set; }
        public DrawingHelper DrawingHelper { get; private set; }
        public AutoPushing AutoPushing { get; private set; }
        public LaneHelper LaneHelper { get; private set; }
        public DamageCalculator DamageCalculator { get; private set; }
        public Config Config { get; private set; }
        public PussyCombo PussyCombo { get; private set; }
        public SpamCombos SpamCombos { get; private set; }
        public InfoPanel InfoPanel { get; private set; }

        public int GetExtraDelay => Config.ExtraDelay.Value.Value;
        public Hero Me;
        //private SentryClient _client;

        protected override void OnActivate()
        {
            Me = EntityManager.LocalHero;
            Config = new Config(this);
            AbilitiesInCombo = new AbilitiesInCombo(this);
            KillSteal = new KillSteal(this);
            DrawingHelper = new DrawingHelper(this);
            AutoPushing = new AutoPushing(this);
            LaneHelper = new LaneHelper(this);
            PussyCombo = new PussyCombo(this);
            SpamCombos = new SpamCombos(this);
            InfoPanel = new InfoPanel(this);
            /*Context.Input.RegisterHotkey("Test", Key.F, args =>
            {
                //Log.Warn($"[{AbilitiesInCombo.Rearm.Ability.Id}] Damage: {AbilitiesInCombo.Rearm.ActivationDelay} {AbilitiesInCombo.Rearm.GetCastDelay()}");
                Log.Warn($"-----------------");
                DamageCalculator.Updater();
            });*/

            DamageCalculator = new DamageCalculator(this);
            var etherialSleeper = new Sleeper();
            /*_client = new SentryClient(
                "https://6b8fedb4d4b942949c4d2a3ed019873f:78d8171a47df490d9d85f7f806b9095b@sentry.io/1545139");
//            _client.Client.Environment = "info";*/

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Console.WriteLine(args);
                var ex = (Exception) args.ExceptionObject;
                //_client.CaptureAsync(ex);
            };
            if (GameManager.GameMode != GameMode.Custom)
            {
                // _client.Capture(new SentryEvent("tinker loaded"));
            }

            Config.ComboKey.PropertyChanged += (sender, args) =>
            {
                if (Config.ComboKey.Value.Active)
                {
                    Hero target = null;
                    UpdateManager.BeginInvoke(async () =>
                    {
                        while (Config.ComboKey.Value.Active)
                        {
                            var notValidTarget = target == null || !target.IsValid || !target.IsAlive;
                            if (!notValidTarget)
                            {
                                Context.Particle.DrawTargetLine(Me, "targetParticle", target.NetworkPosition);
                            }

                            await Task.Delay(25);
                        }

                        Context.Particle.Remove("targetParticle");
                    });
                    UpdateManager.BeginInvoke(async () =>
                    {
                        while (Config.ComboKey.Value.Active)
                        {
                            try
                            {
                                var notValidTarget = target == null || !target.IsValid || !target.IsAlive;
                                if (notValidTarget)
                                {
                                    target = (Hero) Context.TargetSelector.Active.GetTargets().FirstOrDefault();
                                    notValidTarget = target == null || !target.IsValid || !target.IsAlive ||
                                                     !target.IsVisible;
                                    /*await Task.Delay(Config.UpdateRate);
                                    continue;*/
                                }


                                if (Me.IsChanneling() || AbilitiesInCombo.Rearm.Ability.IsInAbilityPhase)
                                {
                                    await Task.Delay(Config.UpdateRate);
                                    continue;
                                }

                                foreach (var activeAbility in DamageCalculator.AllAbilities)
                                {
                                    if (!Config.ComboKey.Value.Active)
                                        break;
                                    if (activeAbility.CanBeCasted)
                                    {
                                        switch (activeAbility)
                                        {
                                            case item_blink blink:
                                                if (Config.UseBlinkOnMousePosition ||
                                                    notValidTarget)
                                                {
                                                    var mousePos = GameManager.MousePosition;
                                                    if (Me.IsInRange(mousePos, blink.CastRange))
                                                    {
                                                        activeAbility.UseAbility(mousePos);
                                                        var extraDelayTime = (int) Math.Max(GameManager.Ping * 1.75f,
                                                            blink.GetCastDelay(mousePos) * 1.15f);
                                                        await Task.Delay(extraDelayTime);
                                                    }
                                                    else
                                                    {
                                                        var pos = Me.Position.Extend(mousePos, blink.CastRange - 50);
                                                        activeAbility.UseAbility(pos);
                                                        var extraDelayTime = (int) Math.Max(GameManager.Ping * 1.75f,
                                                            blink.GetCastDelay(pos) * 1.15f);
                                                        await Task.Delay(extraDelayTime);
                                                    }
                                                }
                                                else if (Config.UseSaveRangeForBlink)
                                                {
                                                    var targetPos = target.Position;
                                                    var mePos = GameManager.MousePosition;
                                                    var pos = targetPos - mePos;
                                                    pos.Normalize();
                                                    pos *= -AbilitiesInCombo.Laser.CastRange +
                                                           Config.RangeForLaserRange;
                                                    pos = targetPos + pos;
                                                    activeAbility.UseAbility(pos);
                                                    var extraDelayTime = (int) Math.Max(GameManager.Ping * 1.75f,
                                                        blink.GetCastDelay(pos) * 1.15f);
                                                    await Task.Delay(extraDelayTime);
                                                    //TODO диреншин
                                                }
                                                else
                                                {
                                                    var targetPos = target.Position;
                                                    var mePos = Me.NetworkPosition;
                                                    if (targetPos.Distance(mePos) <= Config.MinRangeForBlink)
                                                    {
                                                        goto AfterBreak;
                                                    }

                                                    var pos = targetPos - mePos;
                                                    pos.Normalize();
                                                    pos *= -Config.SafeDistance;
                                                    pos = targetPos + pos;
                                                    activeAbility.UseAbility(pos);
                                                    var extraDelayTime = (int) Math.Max(GameManager.Ping * 1.75f,
                                                        blink.GetCastDelay(pos) * 1.15f);
                                                    await Task.Delay(extraDelayTime);
                                                    //await Task.Delay((int)Math.Max(1,GameManager.Ping * 1.2f + GetExtraDelay));
                                                }

                                                if (notValidTarget)
                                                {
                                                    target = (Hero) Context.TargetSelector.Active.GetTargets()
                                                        .FirstOrDefault();
                                                    notValidTarget =
                                                        target == null || !target.IsValid || !target.IsAlive ||
                                                        !target.IsVisible;

                                                    Log.Warn(
                                                        $"Trying to find target, cuz current target is not valid. Not target is valid? {!notValidTarget}");
                                                    /*await Task.Delay(Config.UpdateRate);
                                                    continue;*/
                                                }

                                                break;
                                            case item_ghost _:
                                                activeAbility.UseAbility();
                                                break;
                                            case item_black_king_bar _:
                                                if (notValidTarget)
                                                    goto AfterBreak;
                                                activeAbility.UseAbility();
                                                break;
                                            case item_soul_ring _:
                                                activeAbility.UseAbility();
                                                break;
                                            case item_glimmer_cape _:
                                                activeAbility.UseAbility(Me);
                                                break;
                                            case Rearm _:
                                                var target1 = target;
                                                var count =
                                                    notValidTarget
                                                        ? DamageCalculator.AllAbilities.Where(x =>
                                                            x is item_soul_ring ||
                                                            x is item_glimmer_cape || x is item_ghost).Count(x =>
                                                            !(x is item_blink) && x.CanBeCasted)
                                                        : DamageCalculator.AllAbilities.Count(x =>
                                                            !(x is item_blink) &&
                                                            !(x is item_veil_of_discord veil &&
                                                              target1.HasAnyModifiers(veil.TargetModifierName)) &&
                                                            x.CanBeCasted && x.CanHit(target1));
                                                /*foreach (var ability in DamageCalculator.AllAbilities)
                                                {
                                                    Log.Error(
                                                        $"{ability} CanBeCaster:{ability.CanBeCasted} CanHit:{ability.CanHit(target1)}");
                                                }*/
                                                if (!notValidTarget)
                                                {
                                                    if ((Config.StopComboInBladeMail &&
                                                         target1.HasAnyModifiers("modifier_item_blade_mail_reflect")) ||
                                                        !target.IsVisible)
                                                    {
                                                        var count2 =
                                                            DamageCalculator.AllAbilities.Count(x =>
                                                                x.CanBeCasted && x.CanHit(target1) &&
                                                                x.GetDamage(target1) > 0);
                                                        Log.Error(
                                                            $"[BladeMailFixer] Abilities with dmg {count2} || all: {count}");
                                                        if (count >= count2)
                                                        {
                                                            activeAbility.UseAbility();
                                                            break;
                                                        }
                                                    }
                                                }

                                                if (
                                                    count >=
                                                    1 /* && !(Config.StopComboInBladeMail && target1.HasAnyModifiers("modifier_item_blade_mail_reflect"))*/
                                                )
                                                {
                                                    Log.Error(
                                                        $"Not all skills was casted. {count}. skip rearm");
                                                    continue;
                                                }

                                                activeAbility.UseAbility();
                                                break;
                                            case item_veil_of_discord itemVeilOfDiscord:
                                                if (notValidTarget)
                                                    goto AfterBreak;
                                                if (target.HasModifier(itemVeilOfDiscord.TargetModifierName) ||
                                                    !target.IsVisible ||
                                                    !activeAbility.CanHit(target))
                                                {
                                                    goto AfterBreak;
                                                }

                                                itemVeilOfDiscord.UseAbility(target);
                                                break;
                                            case item_ethereal_blade ethereal:
                                                if (notValidTarget)
                                                    goto AfterBreak;
                                                if (!target.IsVisible)
                                                    goto AfterBreak;
                                                if (ethereal.CanHit(target))
                                                    ethereal.UseAbility(target);

                                                Log.Warn(
                                                    $"[Combo] {activeAbility} {activeAbility.GetCastDelay(target)}");
                                                await Task.Delay(activeAbility.GetCastDelay(target) + 20);

                                                etherialSleeper.Sleep(
                                                    ethereal.GetHitTime(target) + 250 + GetExtraDelay);

                                                while (etherialSleeper.Sleeping &&
                                                       !target.HasModifier(ethereal.TargetModifierName))
                                                {
                                                    await Task.Delay(5);
                                                }

                                                continue;
                                            default:
                                                if (notValidTarget)
                                                    goto AfterBreak;
                                                /*if (activeAbility.Ability.Id == AbilityId.item_dagon ||
                                                    activeAbility.Ability.Id >= AbilityId.item_dagon_2 &&
                                                    activeAbility.Ability.Id <= AbilityId.item_dagon_5)
                                                {
                                                    if (etherialSleeper.Sleeping &&
                                                        !target.HasModifier("modifier_item_ethereal_blade_ethereal"))
                                                        continue;
                                                }*/
                                                if (activeAbility.CanHit(target) && target.IsVisible)
                                                {
                                                    if (Config.StopComboInBladeMail &&
                                                        target.HasModifier("modifier_item_blade_mail_reflect") &&
                                                        activeAbility.GetDamage(target) > 0)
                                                    {
                                                        continue;
                                                    }

                                                    activeAbility.UseAbility(target);
                                                }
                                                else
                                                {
                                                    goto AfterBreak;
                                                }

                                                break;
                                        }

                                        var delayTime =
                                            (activeAbility.Ability.AbilityBehavior & AbilityBehavior.NoTarget) != 0 &&
                                            (activeAbility.Ability.AbilityBehavior & AbilityBehavior.Point) == 0
                                                ? activeAbility.GetCastDelay()
                                                : notValidTarget
                                                    ? activeAbility.GetCastDelay()
                                                    : activeAbility.GetCastDelay(target);
                                        if (!(activeAbility is Rearm) && !(activeAbility is Laser))
                                        {
                                            delayTime -= (int) GameManager.Ping;
                                        }

                                        delayTime = Math.Max(25, delayTime + 20 + GetExtraDelay);
                                        Log.Warn($"[Combo] {activeAbility} {delayTime}");
                                        await Task.Delay(delayTime);
                                        AfterBreak: ;
                                    }
                                    else if (activeAbility is Rearm rearm && Me.Mana <= rearm.ManaCost)
                                    {
                                        Context.Orbwalker.Active.Move(GameManager.MousePosition);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            await Task.Delay(Config.UpdateRate);
                        }
                    });
                }
                else
                {
                    Context.Particle.Remove("targetParticle");
                }
            };


            Config.RearmBlink.PropertyChanged += async (sender, args) =>
            {
                if (Config.RearmBlink.Value.Active)
                {
                    if (AbilitiesInCombo.Blink == null)
                        return;
                    if (AbilitiesInCombo.Rearm.CanBeCasted)
                    {
                        Context.Particle.DrawTargetLine(Me, "RearmBlinkRange",
                            Me.InFront(AbilitiesInCombo.Blink.CastRange - 50));
                        var sleeper = new Sleeper();
                        AbilitiesInCombo.Rearm.UseAbility();
                        await Task.Delay(AbilitiesInCombo.Rearm.GetCastDelay() - 150);
                        sleeper.Sleep(300);
                        while (sleeper.Sleeping || AbilitiesInCombo.Rearm.Ability.IsInAbilityPhase)
                        {
                            if (Config.RearmBlinkSuperSpam)
                            {
                                //TODO дистанция между мышкой и тинкером
                                AbilitiesInCombo.Blink.Ability.UseAbility(
                                    Me.InFront(AbilitiesInCombo.Blink.CastRange - 50));
                            }
                            else
                            {
                                AbilitiesInCombo.Blink.UseAbility(Me.InFront(AbilitiesInCombo.Blink.CastRange - 50));
                            }

                            await Task.Delay(1);
                        }

                        Context.Particle.Remove("RearmBlinkRange");
                    }
                }
            };
        }
    }
}