namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_ultimate_orb)]
    public class UltimateOrb : PassiveAbility
    {
        public UltimateOrb(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}