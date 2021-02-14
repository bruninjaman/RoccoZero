using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_boots_of_elves)]
    public sealed class BandOfElvenskin : PassiveItem
    {
        public BandOfElvenskin(Item item)
            : base(item)
        {
        }
    }
}