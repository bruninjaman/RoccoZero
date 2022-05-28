﻿// <copyright file="slardar_bash.cs" company="Ensage">
//    Copyright (c) 2018 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_slardar
{
    using Ensage.Common.Extensions;

    using Ensage.SDK.Extensions;
    using Ensage.SDK.Abilities.Components;

    public class slardar_bash : PassiveAbility, IHasTargetModifierTexture, IHasModifier, IHasTargetModifier
    {
        public slardar_bash(Ability ability)
            : base(ability)
        {
        }

        public string ModifierName { get; } = "modifier_slardar_bash_active";

        public string TargetModifierName { get; } = "modifier_bashed";

        public string[] TargetModifierTextureName { get; } = { "slardar_bash" };

        public float GetAttackCount
        {
            get
            {
                var modifier = this.Owner.GetModifierByName(this.ModifierName);
                return modifier?.StackCount ?? 0;
            }
        }
    }
}