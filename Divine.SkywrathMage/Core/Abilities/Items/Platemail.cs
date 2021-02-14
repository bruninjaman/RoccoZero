using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_platemail)]
    public sealed class Platemail : PassiveItem
    {
        public Platemail(Item item)
            : base(item)
        {
        }
    }
}