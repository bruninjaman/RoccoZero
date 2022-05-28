﻿// <copyright file="item_tome_of_knowledge.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Game;

    public class item_tome_of_knowledge : ActiveAbility, IItemInfo
    {
        public item_tome_of_knowledge(Item item)
            : base(item)
        {
        }

        public int StockCount
        {
            get
            {
                var itemStockInfo = GameManager.ItemStockInfos.FirstOrDefault(x => x.AbilityId == this.Ability.Id && x.Team == this.Owner.Team);
                return itemStockInfo?.StockCount ?? 0;
            }
        }
    }
}