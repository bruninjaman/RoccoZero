using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_aegis)]
    public sealed class AegisOfTheImmortal : PassiveItem
    {
        public AegisOfTheImmortal(Item item)
            : base(item)
        {
        }
    }
}