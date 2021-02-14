using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_bracer)]
    public sealed class Bracer : PassiveItem
    {
        public Bracer(Item item)
            : base(item)
        {
        }
    }
}