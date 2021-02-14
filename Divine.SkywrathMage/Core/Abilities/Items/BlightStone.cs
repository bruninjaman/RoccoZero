using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_blight_stone)]
    public sealed class BlightStone : PassiveItem
    {
        public BlightStone(Item item)
            : base(item)
        {
        }
    }
}