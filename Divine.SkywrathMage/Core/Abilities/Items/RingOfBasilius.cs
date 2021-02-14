using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ring_of_basilius)]
    public sealed class RingOfBasilius : ToggleItem, IAuraAbility
    {
        public RingOfBasilius(Item item)
            : base(item)
        {
        }

        public string AuraModifierName { get; } = "modifier_item_ring_of_basilius_aura_bonus";

        public float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("aura_radius");
            }
        }
    }
}