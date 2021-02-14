using Divine.SDK.Extensions;
using Divine.SDK.Prediction;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class LineItem : PredictionItem
    {
        protected LineItem(Item item)
            : base(item)
        {
        }

        public override PredictionSkillshotType PredictionSkillshotType { get; } = PredictionSkillshotType.SkillshotLine;

        public override bool CanHit(Unit target)
        {
            return Owner.Distance2D(target) < Range + Radius;
        }
    }
}