using Divine.SDK.Extensions;
using Divine.SDK.Prediction;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class LineSpell : PredictionSpell
    {
        protected LineSpell(Ability ability)
            : base(ability)
        {
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotLine;

        public override bool CanHit(Unit target)
        {
            return Owner.Distance2D(target) < Range + Radius;
        }
    }
}