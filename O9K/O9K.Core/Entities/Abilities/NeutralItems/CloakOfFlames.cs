namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_cloak_of_flames)]
    public class CloakOfFlames : PassiveAbility
    {
        public CloakOfFlames(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}