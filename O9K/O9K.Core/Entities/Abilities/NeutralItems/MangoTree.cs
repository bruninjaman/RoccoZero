namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_mango_tree)]
    public class MangoTree : RangedAbility
    {
        public MangoTree(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}