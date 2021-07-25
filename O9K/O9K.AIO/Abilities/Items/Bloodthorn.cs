using System;
using O9K.Core.Entities.Abilities.Heroes.ArcWarden;

namespace O9K.AIO.Abilities.Items
{
    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;

    internal class Bloodthorn : DisableAbility
    {
        public Bloodthorn(ActiveAbility ability)
            : base(ability)
        {
        }

        protected override bool ChainStun(Unit9 target, bool invulnerability)
        {
            
            var remainingTime = target.GetModifier(this.DisableModifierName).RemainingTime;


            return remainingTime < 0.2;
        }
        
        public string DisableModifierName { get; } = "modifier_bloodthorn_debuff";

    }
}