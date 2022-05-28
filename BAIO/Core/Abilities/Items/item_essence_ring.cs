﻿// <copyright file="item_essence_ring.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class item_essence_ring : ActiveAbility, IHasHealthRestore
    {
        public item_essence_ring(Item item)
            : base(item)
        {
        }

        public string TargetModifierName { get; } = "modifier_item_essence_ring_active";

        public float TotalHealthRestore
        {
            get
            {
                return Ability.GetAbilitySpecialData("health_gain");
            }
        }
    }
}