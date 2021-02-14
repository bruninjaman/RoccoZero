using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_rapier)]
    public sealed class DivineRapier : PassiveItem
    {
        public DivineRapier(Item item)
            : base(item)
        {
        }
    }
}