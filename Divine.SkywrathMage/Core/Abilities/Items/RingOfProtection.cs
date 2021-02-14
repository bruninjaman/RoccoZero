using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ring_of_protection)]
    public sealed class RingOfProtection : PassiveItem
    {
        public RingOfProtection(Item item)
            : base(item)
        {
        }
    }
}