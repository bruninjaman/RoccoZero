using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ring_of_tarrasque)]
    public sealed class RingOfTarrasque : PassiveItem
    {
        public RingOfTarrasque(Item item)
            : base(item)
        {
        }
    }
}