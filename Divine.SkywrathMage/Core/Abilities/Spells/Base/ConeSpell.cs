using Divine.SDK.Extensions;
using Divine.SDK.Prediction;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class ConeSpell : PredictionSpell
    {
        protected ConeSpell(Ability ability)
            : base(ability)
        {
        }

        public virtual float EndRadius
        {
            get
            {
                return Radius;
            }
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotCone;

        public override bool CanHit(Unit target)
        {
            return Owner.Distance2D(target) < Range + EndRadius;
        }
    }
}