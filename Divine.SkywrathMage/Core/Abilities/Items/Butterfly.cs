using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_butterfly)]
    public sealed class Butterfly : PassiveItem
    {
        public Butterfly(Item item)
            : base(item)
        {
        }
    }
}