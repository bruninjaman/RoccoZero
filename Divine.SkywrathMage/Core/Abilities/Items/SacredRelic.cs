using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_relic)]
    public sealed class SacredRelic : PassiveItem
    {
        public SacredRelic(Item item)
            : base(item)
        {
        }
    }
}