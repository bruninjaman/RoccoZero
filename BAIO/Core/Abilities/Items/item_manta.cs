﻿// <copyright file="item_manta.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;

    using Ensage.SDK.Abilities.Components;

    public class item_manta : ActiveAbility, IHasModifier
    {
        public item_manta(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_manta_phase";
    }
}