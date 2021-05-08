namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_giants_ring)]
    public class GiantsRing : PassiveAbility
    {
        public GiantsRing(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}