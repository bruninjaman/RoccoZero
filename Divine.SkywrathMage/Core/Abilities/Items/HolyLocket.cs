using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_holy_locket)]
    public sealed class HolyLocket : PassiveItem
    {
        public HolyLocket(Item item)
            : base(item)
        {
        }
    }
}