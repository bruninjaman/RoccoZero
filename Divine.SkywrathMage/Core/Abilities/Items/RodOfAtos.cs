using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_rod_of_atos)]
    public sealed class RodOfAtos : RangedItem, IHasTargetModifier
    {
        public RodOfAtos(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Rooted;

        public override float Speed { get; } = 1500f;

        public string TargetModifierName { get; } = "modifier_rod_of_atos_debuff";
    }
}