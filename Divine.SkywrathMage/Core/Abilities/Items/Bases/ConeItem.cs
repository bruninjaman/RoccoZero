
using Divine.SDK.Extensions;
using Divine.SDK.Prediction;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class ConeItem : PredictionItem
    {
        protected ConeItem(Item item)
            : base(item)
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