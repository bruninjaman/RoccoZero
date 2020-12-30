namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_quickening_charm)]
    public class QuickeningCharm : PassiveAbility
    {
        public QuickeningCharm(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}