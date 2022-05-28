﻿// <copyright file="item_minotaur_horn.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class item_minotaur_horn : ActiveAbility, IHasModifier
    {
        public item_minotaur_horn(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_black_king_bar_immune";

        public override UnitState AppliesUnitState { get; } = UnitState.MagicImmune;

        public override float Duration
        {
            get
            {
                return Ability.GetAbilitySpecialData("duration");
            }
        }
    }
}