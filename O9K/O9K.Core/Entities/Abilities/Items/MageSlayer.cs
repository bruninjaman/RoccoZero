namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_mage_slayer)]
    public class MageSlayer : PassiveAbility
    {
        public MageSlayer(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}