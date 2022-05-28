﻿// <copyright file="item_ironwood_tree.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Extensions;

    public class item_ironwood_tree : RangedAbility
    {
        public item_ironwood_tree(Item item)
            : base(item)
        {
        }

        public override float Duration
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("tree_duration");
            }
        }
    }
}