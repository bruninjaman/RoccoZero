using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_gloves)]
    public sealed class GlovesOfHaste : PassiveItem
    {
        public GlovesOfHaste(Item item)
            : base(item)
        {
        }
    }
}