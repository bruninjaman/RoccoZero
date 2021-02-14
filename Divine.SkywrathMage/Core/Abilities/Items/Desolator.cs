using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_desolator)]
    public sealed class Desolator : PassiveItem
    {
        public Desolator(Item item)
            : base(item)
        {
        }
    }
}