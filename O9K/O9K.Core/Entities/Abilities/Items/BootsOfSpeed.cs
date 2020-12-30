namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_boots)]
    public class BootsOfSpeed : PassiveAbility
    {
        public BootsOfSpeed(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}