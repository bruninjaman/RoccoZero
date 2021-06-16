namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_possessed_mask)]
    public class PossessedMask : PassiveAbility
    {
        public PossessedMask(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}