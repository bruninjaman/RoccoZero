using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_courier)]
    public sealed class AnimalCourier : PassiveItem
    {
        public AnimalCourier(Item item)
            : base(item)
        {
        }
    }
}