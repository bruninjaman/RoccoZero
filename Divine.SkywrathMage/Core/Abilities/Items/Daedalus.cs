using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_greater_crit)]
    public sealed class Daedalus : PassiveItem
    {
        public Daedalus(Item item)
            : base(item)
        {
        }
    }
}