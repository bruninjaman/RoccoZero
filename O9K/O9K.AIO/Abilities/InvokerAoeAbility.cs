using System;
using System.Collections.Generic;
using O9K.AIO.Modes.Combo;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Units;
using O9K.Core.Logger;

namespace O9K.AIO.Abilities
{
    internal class InvokerAoeAbility : AoeAbility
    {
        public InvokerAoeAbility(ActiveAbility ability) : base(ability)
        {
            
        }
        
        private static bool CheckForModifierTime(Unit9 target, string modifierName)
        {
            var modifier = target.GetModifier(modifierName);
            if (modifier == null)
                return true;
            return modifier.RemainingTime < 1.4;
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

            // if (target.IsRooted && !this.Ability.UnitTargetCast && target.GetImmobilityDuration() <= 0)
            // {
            //     return true;
            // }
            
            if (target.CanMove() && target.Speed > 200 && 
                CheckForModifierTime(target, "modifier_invoker_cold_snap")
                && CheckForModifierTime(target, "modifier_invoker_deafening_blast_knockback"))
            {
                return false;
            }

            return true;
            // if (target.CanMove() && target.Speed > 200 && 
            //     CheckForModifierTime(target, "modifier_invoker_cold_snap")
            //     && CheckForModifierTime(target, "modifier_invoker_deafening_blast_knockback"))
            // {
            //     return false;
            // }
            // return base.ShouldConditionCast(targetManager, menu, usableAbilities);
        }
    }
}