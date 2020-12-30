namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_princes_knife)]
    public class PrincesKnife : PassiveAbility
    {
        public PrincesKnife(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}