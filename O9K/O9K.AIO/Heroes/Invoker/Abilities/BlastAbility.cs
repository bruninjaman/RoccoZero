using System;
using System.Collections.Generic;
using O9K.AIO.Abilities;
using O9K.AIO.Modes.Combo;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Heroes.Invoker;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;

namespace O9K.AIO.Heroes.Invoker.Abilities
{
    internal class BlastAbility : InvokerAoeAbility
    {
        private readonly DeafeningBlast blast;

        public BlastAbility(ActiveAbility ability)
            : base(ability)
        {
            this.blast = (DeafeningBlast) ability;
        }

        // public override bool UseAbility(TargetManager.TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        // {
        //     Console.WriteLine("BlastAbility cast --> 1");
        //     if (!Ability.BaseAbility.Cast(targetManager.Target.Position))
        //     {
        //         return false;
        //     }
        //     Console.WriteLine("BlastAbility cast --> 2");
        //     var delay = Ability.GetCastDelay(targetManager.Target);
        //     comboSleeper.Sleep(delay);
        //     Sleeper.Sleep(delay + 0.5f);
        //     OrbwalkSleeper.Sleep(delay);
        //     return true;
        // }

        public override bool ShouldConditionCast(TargetManager.TargetManager targetManager, IComboModeMenu menu, List<UsableAbility> usableAbilities)
        {
            // var target = targetManager.Target;
            // if (target == null)
            //     return false;
            //
            // // if (target.IsStunned)
            // //     return false;
            //
            // if (!target.HasModifier(
            //     "modifier_invoker_ice_wall_slow_debuff",
            //     "modifier_invoker_chaos_meteor_burn",
            //     "modifier_invoker_cold_snap"))
            //     return false;

            return true;
        }
        protected override bool ChainStun(Unit9 target, bool invulnerability)
        {
            var immobile = invulnerability ? target.GetInvulnerabilityDuration() : target.GetImmobilityDuration();
            if (immobile <= 0)
            {
                return false;
            }

            var hitTime = this.Ability.GetHitTime(target);
            // Console.WriteLine($"immobile {immobile} hitTime {hitTime} {Ability.DisplayName}");
            
            return hitTime > immobile;
        }
    }
}