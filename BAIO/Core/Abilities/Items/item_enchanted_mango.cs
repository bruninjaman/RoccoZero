﻿// <copyright file="item_enchanted_mango.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class item_enchanted_mango : RangedAbility, IHasManaRestore
    {
        public item_enchanted_mango(Item item)
            : base(item)
        {
        }

        public float TotalManaRestore
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("replenish_amount");
            }
        }
    }
}