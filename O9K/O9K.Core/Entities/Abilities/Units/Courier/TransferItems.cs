﻿namespace O9K.Core.Entities.Abilities.Units.Courier;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.courier_transfer_items)]
public class TransferItems : ActiveAbility
{
    public TransferItems(Ability baseAbility)
        : base(baseAbility)
    {
    }
}