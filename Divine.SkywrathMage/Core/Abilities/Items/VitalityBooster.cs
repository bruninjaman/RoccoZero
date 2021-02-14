using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_vitality_booster)]
    public sealed class VitalityBooster : PassiveItem
    {
        public VitalityBooster(Item item)
            : base(item)
        {
        }
    }
}