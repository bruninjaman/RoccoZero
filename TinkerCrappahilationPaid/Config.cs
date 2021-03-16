using System;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.SDK.Menu;

namespace TinkerCrappahilationPaid
{
    public class Config
    {
        private readonly TinkerCrappahilationPaid _main;

        public Config(TinkerCrappahilationPaid main)
        {
            _main = main;
            Factory = MenuFactory.Create("Tinker");
            ComboKey = Factory.Item("Combo Key", new KeyBind('0'));
            PussyComboKey = Factory.Item("Pussy Combo Key", new KeyBind('0'));
            RocketSpam = Factory.Item("Rocket spam Key", new KeyBind('0'));
            MarchSpam = Factory.Item("March spam key", new KeyBind('0'));
            RearmBlink = Factory.Item("Rearm blink", new KeyBind('0'));
            RearmBlinkSuperSpam = Factory.Item("Super blink spam for Rearm blink", true);
            PussyComboKey.Item.SetTooltip("will do all combo from trees where enemies cant catch you");
            DisablePlayerInputWhileCombo = Factory.Item("Disable Player Input While Combo", false);
            DisablePlayerInputWhileKillSteal = Factory.Item("Disable Player Input While KillSteal", true);

            UltAntiFailer = Factory.Item("Rearm Fail Helper", true);
            UltAntiFailer.Item.SetTooltip("will prevent double ult self cast");

            RocketsAntiFail = Factory.Item("Rockets Fail Helper", true);
            RocketsAntiFail.Item.SetTooltip("will prevent rocket self cast, if no enemies are in range");

            ExtraDelay = Factory.Item("Extra delay after each ability in combo (ms)", new Slider(1, -150, 100));

            UpdateRate = Factory.Item("Update rate in combo", new Slider(10, 1, 100));
            UpdateRate.Item.SetTooltip("Less value -> more speed && possible more lags");

            StopComboInBladeMail = Factory.Item("Dont do combo in blade mail", true);

            var underRearm = 0f;
            Player.OnExecuteOrder += (sender, args) =>
            {
                if (args.IsPlayerInput && !args.IsQueued)
                {
                    if (args.Ability != null && args.OrderId != OrderId.UpgradeAbility)
                    {
                        if (args.Ability.Id == AbilityId.tinker_heat_seeking_missile)
                        {
                            if (RocketsAntiFail &&
                                !main.AbilitiesInCombo.Rocket.CanHitAnyEnemy())
                            {
                                TinkerCrappahilationPaid.Log.Info($"[RocketsFailHelper] stop {args.OrderId}");
                                args.Process = false;
                            }
                        }
                        else if (args.Ability.Id == AbilityId.tinker_rearm)
                        {
                            var dif = Math.Abs(GameManager.RawGameTime - underRearm);
                            if (UltAntiFailer && (main.Me.IsChanneling() || dif <= 0.35 || main.AbilitiesInCombo.Rearm.Ability.IsInAbilityPhase))
                            {
                                args.Process = false;
                                TinkerCrappahilationPaid.Log.Info($"[RearmFailHelper] stop");
                            }
                            else
                            {
                                underRearm = GameManager.RawGameTime;
                            }
                        }
                    }
                    else if (args.OrderId == OrderId.Stop || args.OrderId == OrderId.Hold ||
                             args.OrderId == OrderId.MoveLocation || args.OrderId == OrderId.MoveTarget ||
                             args.OrderId == OrderId.AttackLocation || args.OrderId == OrderId.AttackTarget)
                    {
                        underRearm = 0;
                    }
                }
                if (DisablePlayerInputWhileCombo && ComboKey ||
                    DisablePlayerInputWhileKillSteal && main.KillSteal.InAction)
                {
                    if (args.IsPlayerInput)
                    {
                        args.Process = false;
                    }
                }
            };

            var blinkMenu = Factory.Menu("Blink settings");
            //ExtraRangeForBlink = blinkMenu.Item("Extra Range", new Slider(1500, 1, 2000));
            UseBlinkOnMousePosition = blinkMenu.Item("Use blink on mouse position", false);
            UseBlinkOnMousePosition.Item.SetTooltip("if true -> will ignore settings below");
            UseSaveRangeForBlink = blinkMenu.Item("Use blink on laser's cast range", true);
            UseSaveRangeForBlink.Item.SetTooltip("if true -> will ignore settings below");
            RangeForLaserRange = blinkMenu.Item("Extra range for blink", new Slider(200, 1, 800));
            RangeForLaserRange.Item.SetTooltip("Only for Laser's blink setting");
            MinRangeForBlink = blinkMenu.Item("Min Range", new Slider(200, 1, 500));
            MinRangeForBlink.Item.SetTooltip("Will not use blink, if distance to target less then current value");
            SafeDistance = blinkMenu.Item("Safe distance", new Slider(100, 1, 500));
            SafeDistance.Item.SetTooltip("Will use blink on [target's position - selected value]");

            var debugMenu = Factory.Menu("Debug drawing");
            DrawBlinkPoints = debugMenu.Item("Draw safe blink points", false);
            DrawJunglePoints = debugMenu.Item("Draw jungle points", false);
        }

        public MenuItem<Slider> UpdateRate { get; set; }

        public MenuItem<Slider> RangeForLaserRange { get; set; }

        public MenuItem<bool> UseSaveRangeForBlink { get; set; }

        public MenuItem<bool> RearmBlinkSuperSpam { get; set; }

        public MenuItem<KeyBind> RearmBlink { get; set; }

        public MenuItem<KeyBind> MarchSpam { get; set; }

        public MenuItem<KeyBind> RocketSpam { get; set; }

        public MenuItem<KeyBind> PussyComboKey { get; set; }

        public MenuItem<bool> UseBlinkOnMousePosition { get; set; }

        public MenuItem<bool> DrawJunglePoints { get; set; }

        public MenuItem<bool> DrawBlinkPoints { get; set; }

        public MenuItem<bool> StopComboInBladeMail { get; set; }

        public MenuItem<Slider> ExtraDelay { get; set; }

        public MenuItem<bool> RocketsAntiFail { get; set; }

        public MenuItem<bool> UltAntiFailer { get; set; }

        public MenuItem<Slider> SafeDistance { get; set; }

        public MenuItem<Slider> MinRangeForBlink { get; set; }

        //public MenuItem<Slider> ExtraRangeForBlink { get; set; }

        public MenuItem<bool> DisablePlayerInputWhileKillSteal { get; set; }

        public MenuItem<bool> DisablePlayerInputWhileCombo { get; set; }

        public MenuItem<KeyBind> ComboKey { get; set; }

        public MenuItem<PriorityChanger> Priority { get; set; }

        public MenuItem<AbilityToggler> ItemsInCombo { get; set; }

        public MenuFactory Factory { get; set; }
        public MenuItem<AbilityToggler> ItemsInSpamCombo { get; set; }
    }
}