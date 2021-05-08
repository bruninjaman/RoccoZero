namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_quicksilver_amulet)]
    public class QuicksilverAmulet : PassiveAbility
    {
        public QuicksilverAmulet(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}