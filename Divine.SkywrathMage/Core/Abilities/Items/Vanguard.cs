using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_vanguard)]
    public sealed class Vanguard : PassiveItem
    {
        public Vanguard(Item item)
            : base(item)
        {
        }
    }
}