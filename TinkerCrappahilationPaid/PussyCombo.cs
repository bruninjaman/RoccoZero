using System;
using System.Linq;
using System.Threading.Tasks;

using Divine;
using Divine.SDK.Extensions;

using SharpDX;

using TinkerCrappahilationPaid.Abilities;

namespace TinkerCrappahilationPaid
{
    public class PussyCombo
    {
        private readonly TinkerCrappahilationPaid _main;
        private AbilitiesInCombo Abilities => _main.AbilitiesInCombo;
        private Config Config => _main.Config;
        private Hero Me => _main.Me;

        public PussyCombo(TinkerCrappahilationPaid tinkerCrappahilationPaid)
        {
            _main = tinkerCrappahilationPaid;
            Config.PussyComboKey.PropertyChanged += (sender, args) =>
            {
                if (Config.PussyComboKey.Value.Active)
                {
                    UpdateManager.BeginInvoke(async () =>
                    {
                        Hero target = null;
                        while (Config.PussyComboKey.Value.Active)
                        {
                            try
                            {
                                if (target == null || !target.IsValid || !target.IsAlive)
                                {
                                    target = (Hero)_main.Context.TargetSelector.Active.GetTargets().FirstOrDefault();
                                    await Task.Delay(50);
                                    continue;
                                }

                                if (Me.IsChanneling() || Abilities.Rearm.Ability.IsInAbilityPhase)
                                {
                                    await Task.Delay(50);
                                    continue;
                                }

                                if (_main.KillSteal.InAction)
                                {
                                    TinkerCrappahilationPaid.Log.Warn($"[PussyCombo]: skip cuz kill steal in action");
                                    await Task.Delay(50);
                                    continue;
                                }

                                foreach (var activeAbility in _main.DamageCalculator.AllAbilities)
                                {
                                    if (!Config.PussyComboKey.Value.Active)
                                        break;
                                    if (activeAbility.CanBeCasted)
                                    {
                                        switch (activeAbility)
                                        {
                                            case item_blink blink:
                                                var pos = GetSafePosition(target);
                                                if (!pos.IsZero)
                                                {
                                                    TinkerCrappahilationPaid.Log.Debug($"[PussyCombo] found blink position");
                                                    blink.UseAbility(pos);
                                                    await Task.Delay(Math.Max(25, activeAbility.GetCastDelay(pos) + 20 + GetExtraDelay));
                                                    goto AfterBreak;
                                                }
                                                else
                                                {
                                                    TinkerCrappahilationPaid.Log.Debug($"[PussyCombo] cant find any position for blink");
                                                }
                                                break;
                                            case item_ghost _:
                                                activeAbility.UseAbility();
                                                break;
                                            case item_bottle bottle:
                                                if (!Me.HasAnyModifiers(bottle.TargetModifierName))
                                                {
                                                    bottle.UseAbility();
                                                }
                                                break;
                                            case item_soul_ring _:
                                                activeAbility.UseAbility();
                                                break;
                                            case item_glimmer_cape _:
                                                activeAbility.UseAbility(Me);
                                                break;
                                            case Rearm _:
                                                activeAbility.UseAbility();
                                                break;
                                            case item_veil_of_discord itemVeilOfDiscord:
                                                if (target.HasModifier(itemVeilOfDiscord.TargetModifierName) ||
                                                    !activeAbility.CanHit(target))
                                                {
                                                    goto AfterBreak;
                                                }
                                                itemVeilOfDiscord.UseAbility(target);
                                                break;
                                            case Rockets rockets:
                                                if (rockets.CanHit(target))
                                                {
                                                    rockets.UseAbility();
                                                }
                                                break;
                                            default:
                                                continue;
                                        }
                                        var delayTime =
                                            (activeAbility.Ability.AbilityBehavior & AbilityBehavior.NoTarget) != 0 &&
                                            (activeAbility.Ability.AbilityBehavior & AbilityBehavior.Point) == 0
                                                ? activeAbility.GetCastDelay()
                                                : activeAbility.GetCastDelay(target);
                                        TinkerCrappahilationPaid.Log.Warn($"[PussyCombo] {activeAbility} {delayTime}");
                                        await Task.Delay(Math.Max(25, delayTime + 20 + GetExtraDelay));
                                        AfterBreak:;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            await Task.Delay(50);
                        }
                    });
                }
            };
        }

        public int GetExtraDelay => _main.GetExtraDelay;

        private Vector3 GetSafePosition(Hero target)
        {
            var willDie = false;
            if (_main.DamageCalculator.DamageDict.TryGetValue(target, out var value))
            {
                willDie = value.HealthAfterFirstCastWithoutRange < 0;
            }
            var targetPosition = target.Position;
            var positions = _main.AutoPushing.SaveSpotTable;
            var bestPositions = positions
                .Where(x => !x.IsInRange(Me, 250) && Me.IsInRange(x, Abilities.Blink.CastRange - 50) &&
                            targetPosition.IsInRange(x, Abilities.Rocket.CastRange - 250));
            Vector3 bestPosition;
            if (willDie)
            {
                bestPosition= bestPositions
                    .OrderBy(z => z.Distance2D(target.Position)).FirstOrDefault();
            }
            else
            {
                bestPosition = bestPositions
                    .OrderByDescending(z => z.Distance2D(Me.Position)).FirstOrDefault();
            }
            return bestPosition;
        }
    }
}