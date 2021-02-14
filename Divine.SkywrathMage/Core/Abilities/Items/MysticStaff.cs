using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_mystic_staff)]
    public sealed class MysticStaff : PassiveItem
    {
        public MysticStaff(Item item)
            : base(item)
        {
        }
    }
}