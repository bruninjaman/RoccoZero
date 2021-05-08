namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_helm_of_the_dominator)]
    public class HelmOfTheDominator : RangedAbility
    {
        public HelmOfTheDominator(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override bool TargetsEnemy { get; } = false;
    }
}