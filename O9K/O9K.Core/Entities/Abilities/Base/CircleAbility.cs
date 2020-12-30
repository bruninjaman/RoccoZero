namespace O9K.Core.Entities.Abilities.Base
{
    using Divine;

    using Prediction.Data;

    public abstract class CircleAbility : PredictionAbility
    {
        protected CircleAbility(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override SkillShotType SkillShotType { get; } = SkillShotType.Circle;
    }
}