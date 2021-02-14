using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_quarterstaff)]
    public sealed class Quarterstaff : PassiveItem
    {
        public Quarterstaff(Item item)
            : base(item)
        {
        }
    }
}