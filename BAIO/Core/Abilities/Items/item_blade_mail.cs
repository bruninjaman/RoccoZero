// <copyright file="item_blade_mail.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Items
{
    using Divine.Entity.Entities.Abilities.Items;

    using Ensage.SDK.Abilities.Components;

    public class item_blade_mail : ActiveAbility, IHasModifier
    {
        public item_blade_mail(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_blade_mail_reflect";
    }
}