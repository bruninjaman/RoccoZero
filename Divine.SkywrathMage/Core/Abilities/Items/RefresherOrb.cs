using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_refresher)]
    public sealed class RefresherOrb : ActiveItem
    {
        public RefresherOrb(Item item)
            : base(item)
        {
        }
    }
}