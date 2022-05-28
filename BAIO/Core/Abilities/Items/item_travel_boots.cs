﻿// <copyright file="item_travel_boots.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;

    using Ensage.SDK.Abilities.Aggregation;
    using Ensage.SDK.Abilities.Components;

    public class item_travel_boots : TravelBoots, IHasModifierTexture
    {
        public item_travel_boots(Item item)
            : base(item)
        {
        }

        public string[] ModifierTextureName { get; } = { "item_travel_boots", "item_travel_boots_tinker" };
    }
}