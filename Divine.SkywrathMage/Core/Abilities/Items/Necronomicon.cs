using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_necronomicon)]
    [Item(AbilityId.item_necronomicon_2)]
    [Item(AbilityId.item_necronomicon_3)]
    public sealed class Necronomicon : ActiveItem
    {
        public Necronomicon(Item item)
            : base(item)
        {
        }
    }
}