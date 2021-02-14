using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_basher)]
    public sealed class SkullBasher : PassiveItem
    {
        public SkullBasher(Item item)
            : base(item)
        {
        }
    }
}