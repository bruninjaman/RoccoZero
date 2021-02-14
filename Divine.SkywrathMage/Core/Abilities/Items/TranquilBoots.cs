using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_tranquil_boots)]
    public sealed class TranquilBoots : PassiveItem
    {
        public TranquilBoots(Item item)
            : base(item)
        {
        }
    }
}