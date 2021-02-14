using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_point_booster)]
    public sealed class PointBooster : PassiveItem
    {
        public PointBooster(Item item)
            : base(item)
        {
        }
    }
}