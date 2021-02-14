using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_moon_shard)]
    public sealed class MoonShard : PassiveItem
    {
        public MoonShard(Item item)
            : base(item)
        {
        }
    }
}