using System;
using System.Threading.Tasks;

using Divine;
using Divine.SDK.Extensions;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Menu;

namespace TinkerCrappahilationPaid
{
    public class KillSteal
    {
        private readonly TinkerCrappahilationPaid _main;
        private Config Config => _main.Config;
        private Hero Me => _main.Me;
        public KillSteal(TinkerCrappahilationPaid main)
        {
            _main = main;
            KillStealFactory = Config.Factory.Menu("KillSteal");
            Enable = KillStealFactory.Item("Enable", true);
            InAction = false;
            if (Enable)
            {
                UpdateManager.BeginInvoke(KillStealChecker);
            }

            Enable.PropertyChanged += (sender, args) =>
            {
                if (Enable)
                {
                    UpdateManager.BeginInvoke(KillStealChecker);
                }
                else
                {
                    //UpdateManager.Unsubscribe(KillStealChecker);
                }
            };
        }

        private async void KillStealChecker()
        {
            TinkerCrappahilationPaid.Log.Warn($"[KillSteal] Enable");
            while (Enable)
            {
                try
                {
                    if (Me.IsChanneling() || _main.Config.ComboKey.Value.Active)
                    {
                        await Task.Delay(150);
                        continue;
                    }

                    foreach (var t in _main.DamageCalculator.DamageDict)
                    {
                        if (t.Value.WillDieAfterFirstCast)
                        {
                            var stopCombo = Config.StopComboInBladeMail &&
                                            t.Value.Hero.HasModifier("modifier_item_blade_mail_reflect");
                            if (stopCombo && t.Value.Hero.Health > Me.Health)
                            {
                                continue;
                            }
                            TinkerCrappahilationPaid.Log.Debug($"Try to killsteal -> {t.Key.HeroId}");
                            InAction = true;
                            foreach (var ability in t.Value.AbilitiesForKillSteal)
                            {
                                ability.UseAbility(t.Key);
                                var delay = Math.Max((int) GameManager.Ping, ability.GetCastDelay(t.Key) + 5 + Config.ExtraDelay);
                                TinkerCrappahilationPaid.Log.Debug($"[KillSteal] {ability} ({delay} ms)");
                                await Task.Delay(Math.Max(1, delay));
                            }
                            InAction = false;
                            await Task.Delay(150);
                            break;
                        }
                        else
                        {
//                            TinkerCrappahilationPaid.Log.Debug($"Try to killsteal -> {t.Key.HeroId} Health: {t.Value.Health} Damage: {t.Value.DamageTaken}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
                await Task.Delay(150);
            }
            TinkerCrappahilationPaid.Log.Warn($"[KillSteal] Disable");
        }

        public MenuItem<bool> Enable { get; set; }

        public MenuFactory KillStealFactory { get; set; }
        public bool InAction { get; set; }
    }
}