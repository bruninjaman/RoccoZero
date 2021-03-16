using System;
using System.Threading.Tasks;

using Divine;

namespace TinkerCrappahilationPaid
{
    public class SpamCombos
    {
        private bool CheckItem(string s)
        {
            return Config.ItemsInSpamCombo.Value.IsEnabled(s);
        }

        public SpamCombos(TinkerCrappahilationPaid main)
        {
            _main = main;
            var inRockets = false;
            var inMarch = false;
            Config.RocketSpam.PropertyChanged += async (sender, args) =>
            {
                if (Config.RocketSpam && !inRockets)
                {
                    inRockets = true;
                    while (Config.RocketSpam)
                    {
                        try
                        {
                            if (Config.MarchSpam || Abilities.Rearm.Ability.IsInAbilityPhase || Abilities.Rearm.Ability.IsChanneling)
                            {
                                await Task.Delay(50);
                                continue;
                            }
                            if (Abilities.Blink != null && Abilities.Blink.CanBeCasted && CheckItem(Abilities.Blink.ToString()))
                            {
                                var mousePos = GameManager.MousePosition;
                                var myPos = Me.NetworkPosition;
                                if (Me.IsInRange(mousePos, Abilities.Blink.CastRange))
                                {
                                    if (mousePos.Distance2D(myPos) >= 200)
                                    {
                                        Abilities.Blink.UseAbility(mousePos);
                                        await Task.Delay(
                                            (int) Math.Max(1,
                                                Abilities.Blink.GetCastDelay(mousePos) * 1.1f + _main.GetExtraDelay));
                                    }
                                }
                                else
                                {
                                    var pos = Me.Position.Extend(mousePos, Abilities.Blink.CastRange - 50);
                                    if (pos.Distance2D(myPos) >= 200)
                                    {
                                        Abilities.Blink.UseAbility(pos);
                                        await Task.Delay(
                                            (int) Math.Max(1,
                                                Abilities.Blink.GetCastDelay(pos) * 1.1f + _main.GetExtraDelay));
                                    }
                                }
                            }

                            if (Abilities.Bottle != null && Abilities.Bottle.CanBeCasted && CheckItem(Abilities.Bottle.ToString()))
                            {
                                Abilities.Bottle.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Bottle.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.SoulRing != null && Abilities.SoulRing.CanBeCasted && CheckItem(Abilities.SoulRing.ToString()))
                            {
                                Abilities.SoulRing.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.SoulRing.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.Ghost != null && Abilities.Ghost.CanBeCasted && CheckItem(Abilities.Ghost.ToString()))
                            {
                                Abilities.SoulRing.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Ghost.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.Glimmer != null && Abilities.Glimmer.CanBeCasted && CheckItem(Abilities.Glimmer.ToString()))
                            {
                                Abilities.Glimmer.UseAbility(Me);
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Glimmer.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.Shiva != null && Abilities.Shiva.CanBeCasted && CheckItem(Abilities.Shiva.ToString()))
                            {
                                Abilities.Shiva.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Shiva.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.Rocket.CanBeCasted)
                            {
                                Abilities.Rocket.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Rocket.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.Rearm.CanBeCasted && Config.RocketSpam)
                            {
                                Abilities.Rearm.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Rearm.GetCastDelay() + _main.GetExtraDelay));
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        await Task.Delay(50);
                    }
                    inRockets = false;
                }
                
            };

            Config.MarchSpam.PropertyChanged += async (sender, args) =>
            {
                if (Config.MarchSpam && !inMarch)
                {
                    inMarch = true;
                    while (Config.MarchSpam)
                    {
                        try
                        {
                            if (Abilities.Rearm.Ability.IsInAbilityPhase || Abilities.Rearm.Ability.IsChanneling)
                            {
                                await Task.Delay(50);
                                continue;
                            }

                            if (Abilities.Blink != null && Abilities.Blink.CanBeCasted && CheckItem(Abilities.Blink.ToString()))
                            {
                                var mousePos = GameManager.MousePosition;
                                var myPos = Me.NetworkPosition;
                                if (Me.IsInRange(mousePos, Abilities.Blink.CastRange))
                                {
                                    if (mousePos.Distance2D(myPos) >= 200)
                                    {
                                        Abilities.Blink.UseAbility(mousePos);
                                        await Task.Delay(
                                            (int) Math.Max(1,
                                                Abilities.Blink.GetCastDelay(mousePos) * 1.1f + _main.GetExtraDelay));
                                    }
                                }
                                else
                                {
                                    var pos = Me.Position.Extend(mousePos, Abilities.Blink.CastRange - 50);
                                    if (pos.Distance2D(myPos) >= 200)
                                    {
                                        Abilities.Blink.UseAbility(pos);
                                        await Task.Delay(
                                            (int) Math.Max(1,
                                                Abilities.Blink.GetCastDelay(pos) * 1.1f + _main.GetExtraDelay));
                                    }
                                }
                            }

                            if (Abilities.Bottle != null && Abilities.Bottle.CanBeCasted && CheckItem(Abilities.Bottle.ToString()))
                            {
                                Abilities.Bottle.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Bottle.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.SoulRing != null && Abilities.SoulRing.CanBeCasted && CheckItem(Abilities.SoulRing.ToString()))
                            {
                                Abilities.SoulRing.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.SoulRing.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.Ghost != null && Abilities.Ghost.CanBeCasted && CheckItem(Abilities.Ghost.ToString()))
                            {
                                Abilities.SoulRing.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Ghost.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.Glimmer != null && Abilities.Glimmer.CanBeCasted && CheckItem(Abilities.Glimmer.ToString()))
                            {
                                Abilities.Glimmer.UseAbility(Me);
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Glimmer.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Abilities.Shiva != null && Abilities.Shiva.CanBeCasted && CheckItem(Abilities.Shiva.ToString()))
                            {
                                Abilities.Shiva.UseAbility();
                                await Task.Delay(
                                    (int)Math.Max(1, Abilities.Shiva.GetCastDelay() + _main.GetExtraDelay));
                            }

                            if (Config.RocketSpam)
                            {
                                if (Abilities.Rocket.CanBeCasted)
                                {
                                    Abilities.Rocket.UseAbility();
                                    await Task.Delay(
                                        (int)Math.Max(1, Abilities.Rocket.GetCastDelay() + _main.GetExtraDelay));
                                }
                            }

                            if (Abilities.March.CanBeCasted())
                            {
                                var mousePos = GameManager.MousePosition;
                                if (Me.IsInRange(mousePos, Abilities.March.CastRange))
                                {
                                    if (!Abilities.March.IsInAbilityPhase)
                                    {
                                        Abilities.March.UseAbility(mousePos);
                                        Me.Stop(true);
                                        await Task.Delay(800);
                                    }
                                }
                                else
                                {
                                    var pos = Me.Position.Extend(mousePos, Abilities.Blink.CastRange - 50);
                                    if (!Abilities.March.IsInAbilityPhase)
                                    {
                                        Abilities.March.UseAbility(pos);
                                        Me.Stop(true);
                                        await Task.Delay(800);
                                    }
                                }
                            }

                            if (Abilities.Rearm.CanBeCasted && !Abilities.March.CanBeCasted() && Config.MarchSpam)
                            {
                                Abilities.Rearm.UseAbility();
                                await Task.Delay(
                                    (int) Math.Max(1, Abilities.Rearm.GetCastDelay() + _main.GetExtraDelay));
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        await Task.Delay(50);
                    }
                    inMarch = false;
                }
            };
        }

        private AbilitiesInCombo Abilities => _main.AbilitiesInCombo;
        private Config Config => _main.Config;
        private Hero Me => _main.Me;
        private readonly TinkerCrappahilationPaid _main;
    }
}