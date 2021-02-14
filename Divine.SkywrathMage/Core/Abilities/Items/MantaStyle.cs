using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_manta)]
    public sealed class MantaStyle : ActiveItem, IHasModifier
    {
        public MantaStyle(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_manta_phase";
    }
}