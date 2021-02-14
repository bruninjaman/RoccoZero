using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_broadsword)]
    public sealed class Broadsword : PassiveItem
    {
        public Broadsword(Item item)
            : base(item)
        {
        }
    }
}