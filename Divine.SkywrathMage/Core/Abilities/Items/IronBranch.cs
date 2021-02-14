using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_branches)]
    public sealed class IronBranch : PassiveItem
    {
        public IronBranch(Item item)
            : base(item)
        {
        }
    }
}