namespace O9K.Core.Entities.Abilities.Units.Courier;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.courier_transfer_items_to_other_player)]
public class TransferItemsToOtherPlayer : RangedAbility
{
    public TransferItemsToOtherPlayer(Ability baseAbility)
        : base(baseAbility)
    {
    }
}