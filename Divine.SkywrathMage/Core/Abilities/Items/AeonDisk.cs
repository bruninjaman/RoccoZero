using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_aeon_disk)]
    public sealed class AeonDisk : PassiveItem, IHasModifier
    {
        public AeonDisk(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_aeon_disk_buff";
    }
}