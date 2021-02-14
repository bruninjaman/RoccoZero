using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_maelstrom)]
    public sealed class Maelstrom : PassiveItem
    {
        public Maelstrom(Item item)
            : base(item)
        {
        }
    }
}