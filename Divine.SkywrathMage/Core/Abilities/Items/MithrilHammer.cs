using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_mithril_hammer)]
    public sealed class MithrilHammer : PassiveItem
    {
        public MithrilHammer(Item item)
            : base(item)
        {
        }
    }
}