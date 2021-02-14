using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_aether_lens)]
    public sealed class AetherLens : PassiveItem
    {
        public AetherLens(Item item)
            : base(item)
        {
        }
    }
}