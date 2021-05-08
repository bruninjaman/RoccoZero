namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_chipped_vest)]
    public class ChippedVest : PassiveAbility
    {
        public ChippedVest(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}