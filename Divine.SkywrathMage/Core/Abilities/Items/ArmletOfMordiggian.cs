using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_armlet)]
    public sealed class ArmletOfMordiggian : ToggleItem, IHasModifier
    {
        public ArmletOfMordiggian(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_armlet_unholy_strength";
    }
}