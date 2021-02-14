using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_kaya_and_sange)]
    public sealed class KayaAndSange : PassiveItem // TODO DamageAmplify
    {
        public KayaAndSange(Item item)
            : base(item)
        {
        }
    }
}