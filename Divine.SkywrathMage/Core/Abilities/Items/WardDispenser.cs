using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ward_dispenser)]
    public sealed class WardDispenser : RangedItem
    {
        public WardDispenser(Item item)
            : base(item)
        {
        }

        public uint ObserversCount
        {
            get
            {
                return CurrentCharges;
            }
        }

        public uint SentriesCount
        {
            get
            {
                return SecondaryCharges;
            }
        }
    }
}