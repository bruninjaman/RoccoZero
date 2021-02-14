using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_eagle)]
    public sealed class Eaglesong : PassiveItem
    {
        public Eaglesong(Item item)
            : base(item)
        {
        }
    }
}