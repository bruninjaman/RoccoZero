using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_abyssal_blade)]
    public sealed class AbyssalBlade : RangedItem, IHasTargetModifierTexture
    {
        public AbyssalBlade(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public string[] TargetModifierTextureName { get; } = { "item_abyssal_blade" };
    }
}