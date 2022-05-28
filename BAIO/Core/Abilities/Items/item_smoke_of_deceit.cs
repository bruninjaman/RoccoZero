﻿// <copyright file="item_smoke_of_deceit.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Extensions;
    using Divine.Game;

    using Ensage.SDK.Abilities.Components;

    public class item_smoke_of_deceit : ActiveAbility, IAreaOfEffectAbility, IItemInfo, IHasModifier
    {
        public item_smoke_of_deceit(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_smoke_of_deceit";

        public float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("application_radius");
            }
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