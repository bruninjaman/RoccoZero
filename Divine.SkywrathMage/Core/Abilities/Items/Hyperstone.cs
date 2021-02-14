using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_hyperstone)]
    public sealed class Hyperstone : PassiveItem
    {
        public Hyperstone(Item item)
            : base(item)
        {
        }
    }
}