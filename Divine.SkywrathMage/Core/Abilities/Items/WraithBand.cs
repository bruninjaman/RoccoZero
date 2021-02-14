using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_wraith_band)]
    public sealed class WraithBand : PassiveItem
    {
        public WraithBand(Item item)
            : base(item)
        {
        }
    }
}