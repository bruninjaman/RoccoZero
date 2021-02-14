using Divine.SDK.Extensions;
using Divine.SDK.Prediction;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class CircleSpell : PredictionSpell
    {
        protected CircleSpell(Ability ability)
            : base(ability)
        {
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotCircle;

        public override float Speed { get; } = float.MaxValue;

        public override bool CanHit(Unit target)
        {
            return Owner.Distance2D(target) < (CastRange + Radius);
        }
    }
}