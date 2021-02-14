using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_oblivion_staff)]
    public sealed class OblivionStaff : PassiveItem
    {
        public OblivionStaff(Item item)
            : base(item)
        {
        }
    }
}