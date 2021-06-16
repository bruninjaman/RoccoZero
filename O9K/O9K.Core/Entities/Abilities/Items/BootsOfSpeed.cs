namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

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