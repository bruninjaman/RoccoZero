using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_lesser_crit)]
    public sealed class Crystalys : PassiveItem
    {
        public Crystalys(Item item)
            : base(item)
        {
        }
    }
}