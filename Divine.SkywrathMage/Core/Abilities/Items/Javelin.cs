﻿using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_javelin)]
    public sealed class Javelin : PassiveItem
    {
        public Javelin(Item item)
            : base(item)
        {
        }
    }
}