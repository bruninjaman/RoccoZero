using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_chainmail)]
    public sealed class Chainmail : PassiveItem
    {
        public Chainmail(Item item)
            : base(item)
        {
        }
    }
}