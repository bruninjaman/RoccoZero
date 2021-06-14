using System.Collections.Generic;
using System.Linq;
using O9K.AIO.Abilities;
using O9K.AIO.Modes.Combo;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Items;
using O9K.Core.Logger;

namespace O9K.AIO.Heroes.Invoker.Abilities
{
    internal class RefresherAbility : UntargetableAbility
    {
        private readonly RefresherOrb refresher;

        public RefresherAbility(ActiveAbility ability)
            : base(ability)
        {
            this.refresher = (RefresherOrb) ability;
        }
        
        public override bool ShouldConditionCast(TargetManager.TargetManager targetManager, IComboModeMenu menu, List<UsableAbility> usableAbilities)
        {
            var target = targetManager.Target;
            if (target == null)
                return false;

            var allOnCooldown = usableAbilities.All(x => x.Ability.RemainingCooldown > 2f);
            
            return allOnCooldown && base.ShouldConditionCast(targetManager, menu, usableAbilities);
        }
    }
}