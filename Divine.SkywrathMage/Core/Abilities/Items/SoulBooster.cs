using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_soul_booster)]
    public sealed class SoulBooster : PassiveItem
    {
        public SoulBooster(Item item)
            : base(item)
        {
        }
    }
}