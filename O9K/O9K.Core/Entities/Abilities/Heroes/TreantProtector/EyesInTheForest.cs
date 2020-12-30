namespace O9K.Core.Entities.Abilities.Heroes.TreantProtector
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.treant_eyes_in_the_forest)]
    public class EyesInTheForest : RangedAbility
    {
        public EyesInTheForest(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override bool TargetsEnemy { get; } = false;
    }
}