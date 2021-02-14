using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_vladmir)]
    public sealed class VladmirsOffering : AuraItem
    {
        public VladmirsOffering(Item item)
            : base(item)
        {
        }

        public override string AuraModifierName { get; } = "modifier_item_vladmir_aura";
    }
}