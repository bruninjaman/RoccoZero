﻿// <copyright file="item_satanic.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Extensions;
    using Ensage.SDK.Abilities.Components;

    public class item_satanic : ActiveAbility, IHasModifier
    {
        public item_satanic(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_satanic_unholy";
    }
}