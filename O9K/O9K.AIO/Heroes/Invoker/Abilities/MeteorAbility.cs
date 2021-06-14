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
    internal class MeteorAbility : InvokerAoeAbility
    {
        private readonly ChaosMeteor meteor;

        public MeteorAbility(ActiveAbility ability)
            : base(ability)
        {
            this.meteor = (ChaosMeteor) ability;
        }
        
        public override bool UseAbility(TargetManager.TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            if (!meteor.IsInvoked)
            {
                if (!meteor.CanBeInvoked)
                {
                    return false;
                }
                meteor.Invoke();
            }
            if (!Ability.BaseAbility.Cast(targetManager.Target.Position))
            {
                return false;
            }
            var delay = Ability.GetCastDelay(targetManager.Target) + Ability.ActivationDelay;
            // comboSleeper.Sleep(delay);
            // Sleeper.Sleep(delay + 0.5f);
            comboSleeper.Sleep(0.1f);
            Sleeper.Sleep(0.1f);
            OrbwalkSleeper.Sleep(delay);
            return true;
        }
        
        public override bool ShouldConditionCast(TargetManager.TargetManager targetManager, IComboModeMenu menu, List<UsableAbility> usableAbilities)
        {
            var target = targetManager.Target;
            if (target == null)
                return false;
            if (this.Ability.UnitTargetCast && !target.IsVisible)
            {
                return false;
            }

            if (target.IsMagicImmune && !this.Ability.PiercesMagicImmunity(target))
            {
                return false;
            }

            if (target.IsInvulnerable)
            {
                if (this.Ability.UnitTargetCast)
                {
                    return false;
                }

                if (!this.ChainStun(target, true))
                {
                    return false;
                }
            }

            if (target.IsRooted && !this.Ability.UnitTargetCast && target.GetImmobilityDuration() <= 0)
            {
                return true;
            }
            
            // if (target.CanMove() && target.Speed > 200 && 
            //     CheckForModifierTime(target, "modifier_invoker_cold_snap")
            //     && CheckForModifierTime(target, "modifier_invoker_deafening_blast_knockback"))
            // {
            //     return false;
            // }

            return true;
            // if (target.CanMove() && target.Speed > 200 && 
            //     CheckForModifierTime(target, "modifier_invoker_cold_snap")
            //     && CheckForModifierTime(target, "modifier_invoker_deafening_blast_knockback"))
            // {
            //     return false;
            // }
            // return base.ShouldConditionCast(targetManager, menu, usableAbilities);
        }
        
        
        protected override bool ChainStun(Unit9 target, bool invulnerability)
        {
            var immobile = target.GetInvulnerabilityDuration();
            if (immobile <= 0)
            {
                return false;
            }
            var hitTime = this.Ability.GetCastDelay(target) + this.Ability.ActivationDelay;
            // {
            //     hitTime -= 0.1f;
            // }
            // Console.WriteLine($"immobile {immobile} hitTime {hitTime} {Ability.DisplayName}");

            return hitTime > immobile;
        }
    }
}