namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_ward_sentry)]
    public class SentryWard : RangedAbility
    {
        public SentryWard(Ability ability)
            : base(ability)
        {
        }
    }
}