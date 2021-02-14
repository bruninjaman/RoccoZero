using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_orb_of_venom)]
    public sealed class OrbOfVenom : PassiveItem
    {
        public OrbOfVenom(Item item)
            : base(item)
        {
        }

        public override DamageType DamageType { get; } = DamageType.Magical;
    }
}