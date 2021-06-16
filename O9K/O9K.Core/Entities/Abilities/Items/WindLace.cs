namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_wind_lace)]
    public class WindLace : PassiveAbility
    {
        public WindLace(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}