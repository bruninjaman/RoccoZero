using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_heart)]
    public sealed class HeartOfTarrasque : PassiveItem
    {
        public HeartOfTarrasque(Item item)
            : base(item)
        {
        }
    }
}