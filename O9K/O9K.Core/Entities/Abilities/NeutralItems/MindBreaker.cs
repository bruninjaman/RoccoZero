namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_mind_breaker)]
    public class MindBreaker : PassiveAbility
    {
        public MindBreaker(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}