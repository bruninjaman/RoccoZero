using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_monkey_king_bar)]
    public sealed class MonkeyKingBar : PassiveItem
    {
        public MonkeyKingBar(Item item)
            : base(item)
        {
        }
    }
}