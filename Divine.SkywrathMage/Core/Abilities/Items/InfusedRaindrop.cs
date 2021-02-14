using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_infused_raindrop)]
    public sealed class InfusedRaindrop : PassiveItem //TODO DamageBlock
    {
        public InfusedRaindrop(Item item)
            : base(item)
        {
        }
    }
}