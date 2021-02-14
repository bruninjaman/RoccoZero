using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_diffusal_blade)]
    public sealed class DiffusalBlade : RangedItem, IHasTargetModifier //TODO DamageIncrease
    {
        public DiffusalBlade(Item item)
            : base(item)
        {
        }

        public string TargetModifierName { get; } = "modifier_item_diffusal_blade_slow";
    }
}