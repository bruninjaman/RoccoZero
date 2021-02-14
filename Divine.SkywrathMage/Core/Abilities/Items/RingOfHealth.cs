using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ring_of_health)]
    public sealed class RingOfHealth : PassiveItem
    {
        public RingOfHealth(Item item)
            : base(item)
        {
        }
    }
}