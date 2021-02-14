using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_blade_of_alacrity)]
    public sealed class BladeOfAlacrity : PassiveItem
    {
        public BladeOfAlacrity(Item item)
            : base(item)
        {
        }
    }
}