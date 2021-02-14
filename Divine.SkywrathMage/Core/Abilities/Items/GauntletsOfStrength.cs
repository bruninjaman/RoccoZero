﻿using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_gauntlets)]
    public sealed class GauntletsOfStrength : PassiveItem
    {
        public GauntletsOfStrength(Item item)
            : base(item)
        {
        }
    }
}