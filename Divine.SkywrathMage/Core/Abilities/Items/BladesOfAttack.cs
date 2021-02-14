using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_blades_of_attack)]
    public sealed class BladesOfAttack : PassiveItem
    {
        public BladesOfAttack(Item item)
            : base(item)
        {
        }
    }
}