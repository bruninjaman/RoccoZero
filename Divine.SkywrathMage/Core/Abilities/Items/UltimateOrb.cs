using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ultimate_orb)]
    public sealed class UltimateOrb : PassiveItem
    {
        public UltimateOrb(Item item)
            : base(item)
        {
        }
    }
}