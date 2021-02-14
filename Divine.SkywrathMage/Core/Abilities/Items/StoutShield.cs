using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_stout_shield)]
    public sealed class StoutShield : PassiveItem
    {
        public StoutShield(Item item)
            : base(item)
        {
        }
    }
}