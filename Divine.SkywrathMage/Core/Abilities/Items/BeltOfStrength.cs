using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_belt_of_strength)]
    public sealed class BeltOfStrength : PassiveItem
    {
        public BeltOfStrength(Item item)
            : base(item)
        {
        }
    }
}