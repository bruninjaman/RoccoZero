using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_heavens_halberd)]
    public sealed class HeavensHalberd : RangedItem, IHasTargetModifier
    {
        public HeavensHalberd(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Disarmed;

        public string TargetModifierName { get; } = "modifier_heavens_halberd_debuff";
    }
}