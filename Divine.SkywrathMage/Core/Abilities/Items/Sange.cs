using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_sange)]
    public sealed class Sange : PassiveItem
    {
        public Sange(Item item)
            : base(item)
        {
        }
    }
}