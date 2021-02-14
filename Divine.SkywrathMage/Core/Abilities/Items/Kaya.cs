using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_kaya)]
    public sealed class Kaya : PassiveItem // TODO DamageAmplify
    {
        public Kaya(Item item)
            : base(item)
        {
        }
    }
}