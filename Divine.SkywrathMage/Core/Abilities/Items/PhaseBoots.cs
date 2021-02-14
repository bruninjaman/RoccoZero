using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_phase_boots)]
    public sealed class PhaseBoots : PassiveItem, IHasModifier
    {
        public PhaseBoots(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_phase_boots_active";
    }
}