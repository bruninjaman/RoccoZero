using System;
using System.Collections.Generic;
using Divine.SDK.Helpers;
using O9K.AIO.Abilities;
using O9K.AIO.Modes.Combo;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Heroes.Invoker;
using O9K.Core.Entities.Units;
using O9K.Core.Logger;
using Sleeper = O9K.Core.Helpers.Sleeper;

namespace O9K.AIO.Heroes.Invoker.Abilities
{
    internal class SunStrikeAbility : InvokerAoeAbility
    {
        private readonly SunStrike sunStrike;

        public SunStrikeAbility(ActiveAbility ability) : base(ability)
        {
            sunStrike = (SunStrike) ability;
        }

        public override bool ShouldConditionCast(TargetManager.TargetManager targetManager, IComboModeMenu menu, List<UsableAbility> usableAbilities)
        {
            var shouldCast = base.ShouldConditionCast(targetManager, menu, usableAbilities);
            // Logger.Warn($"ShouldConditionCast {Ability.DisplayName} {shouldCast}");
            return shouldCast;
        }

        protected override bool ChainStun(Unit9 target, bool invulnerability)
        {
            var immobile = target.GetImmobilityDuration();
            if (immobile <= 0)
            {
                immobile = target.GetInvulnerabilityDuration();
                if (immobile <= 0)
                {
                    return false;
                }
            }


            var hitTime = Ability.GetHitTime(target) /*+ .1f*/;

            if (target.IsInvulnerable)
            {
                hitTime -= 0.1f;
            }

            return hitTime > immobile;
            Logger.Warn($"ChainStun {Ability.DisplayName} {base.ChainStun(target, invulnerability)}");
            return base.ChainStun(target, invulnerability);
        }

        public override bool UseAbility(TargetManager.TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            if (!Ability.BaseAbility.Cast(targetManager.Target.Position))
            {
                return false;
            }
            var delay = Ability.GetCastDelay(targetManager.Target);
            comboSleeper.Sleep(delay);
            Sleeper.Sleep(delay + 0.5f);
            OrbwalkSleeper.Sleep(delay);
            return true;
        }

        // public override bool UseAbility(TargetManager.TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        // {
        //     Console.WriteLine($"UseAbility. {Ability.DisplayName} -> 1");
        //     if (!this.Ability.UseAbility(targetManager.Target, targetManager.EnemyHeroes, HitChance.Low))
        //     {
        //         return false;
        //     }
        //     Console.WriteLine($"UseAbility. {Ability.DisplayName} -> 2");
        //     var delay = this.Ability.GetCastDelay(targetManager.Target);
        //     comboSleeper.Sleep(delay);
        //     this.Sleeper.Sleep(delay + 0.5f);
        //     this.OrbwalkSleeper.Sleep(delay);
        //     return true;
        // }
    }
}