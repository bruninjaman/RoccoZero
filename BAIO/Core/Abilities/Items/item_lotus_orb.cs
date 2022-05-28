// <copyright file="item_lotus_orb.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;

    using Ensage.SDK.Abilities.Components;

    public class item_lotus_orb : RangedAbility, IHasTargetModifier
    {
        public item_lotus_orb(Item item)
            : base(item)
        {
        }

        public string TargetModifierName { get; } = "modifier_item_lotus_orb_active";
    }
}