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
            if (target.CanMove() && target.Speed > 200 && 
                CheckForModifierTime(target, "modifier_invoker_cold_snap")
                && CheckForModifierTime(target, "modifier_invoker_deafening_blast_knockback"))
            {
                return false;
            }
            return base.ShouldConditionCast(targetManager, menu, usableAbilities);
        }

        protected override bool ChainStun(Unit9 target, bool invulnerability)
        {
            var immobile = target.GetInvulnerabilityDuration();
            if (immobile <= 0)
            {
                return false;
            }
            var hitTime = this.Ability.GetCastDelay(target) + this.Ability.ActivationDelay;
            {
                hitTime -= 0.1f;
            }

            return hitTime > immobile;
        }
    }
}