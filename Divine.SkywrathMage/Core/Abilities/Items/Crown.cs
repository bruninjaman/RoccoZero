using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_crown)]
    public sealed class Crown : PassiveItem
    {
        public Crown(Item item)
            : base(item)
        {
        }
    }
}