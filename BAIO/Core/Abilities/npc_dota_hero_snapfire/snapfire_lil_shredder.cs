﻿namespace Ensage.SDK.Abilities.npc_dota_hero_snapfire
{
    using BAIO.Core.Extensions;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class snapfire_lil_shredder : ActiveAbility, IHasModifier, IHasTargetModifier
    {
        public snapfire_lil_shredder(Ability ability)
            : base(ability)
        {
        }

        protected override float RawDamage
        {
            get
            {
                var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_snapfire_6);
                return talent?.Level > 0 ? Owner.DamageAverage : this.Ability.GetAbilitySpecialData("damage");
            }
        }

        public int ModifierCount
        {
            get
            {
                return Owner.HasModifier(ModifierName) ? Owner.FindModifier(ModifierName).StackCount : 0;
            }
        }

        public string ModifierName { get; } = "modifier_snapfire_lil_shredder_buff";
        public string TargetModifierName { get; } = "modifier_snapfire_lil_shredder_debuff";
    }
}