﻿// <copyright file="ember_spirit_fire_remnant.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_ember_spirit
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class ember_spirit_fire_remnant : LineAbility, IHasModifier
    {
        public ember_spirit_fire_remnant(Ability ability)
            : base(ability)
        {
            var activeAbility = this.Owner.GetAbilityById(AbilityId.ember_spirit_activate_fire_remnant);
            this.ActivateRemnantAbility = new ember_spirit_activate_fire_remnant(activeAbility);
        }

        public ember_spirit_activate_fire_remnant ActivateRemnantAbility { get; }

        public override float Duration
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("duration");
            }
        }

        public string ModifierName { get; } = "modifier_ember_spirit_fire_remnant_charge_counter";

        public override float Speed
        {
            get
            {
                return this.Owner.HasAghanimsScepter() ?
                    ((this.Ability.GetAbilitySpecialData("speed_multiplier") / 100) * this.Owner.MovementSpeed) * this.Ability.GetAbilitySpecialData("scepter_speed_multiplier")
                    : (this.Ability.GetAbilitySpecialData("speed_multiplier") / 100) * this.Owner.MovementSpeed;
            }
        }
    }
}