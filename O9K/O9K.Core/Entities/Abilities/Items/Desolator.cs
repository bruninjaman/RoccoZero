namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_desolator)]
    public class Desolator : PassiveAbility
    {
        public Desolator(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}