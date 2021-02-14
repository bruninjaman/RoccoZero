using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_yasha_and_kaya)]
    public sealed class YashaAndKaya : PassiveItem // TODO DamageAmplify
    {
        public YashaAndKaya(Item item)
            : base(item)
        {
        }
    }
}