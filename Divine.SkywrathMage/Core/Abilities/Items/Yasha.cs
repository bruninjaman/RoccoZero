using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_yasha)]
    public sealed class Yasha : PassiveItem
    {
        public Yasha(Item item)
            : base(item)
        {
        }
    }
}