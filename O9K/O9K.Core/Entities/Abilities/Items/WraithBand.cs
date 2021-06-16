namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_wraith_band)]
    public class WraithBand : PassiveAbility
    {
        public WraithBand(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}