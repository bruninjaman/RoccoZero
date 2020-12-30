namespace O9K.Core.Entities.Abilities.Base
{
    using Divine;

    using Prediction.Data;

    public abstract class LineAbility : PredictionAbility
    {
        protected LineAbility(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override SkillShotType SkillShotType { get; } = SkillShotType.Line;
    }
}