namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

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