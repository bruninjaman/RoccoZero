using Divine.SDK.Extensions;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class AreaOfEffectItem : RangedItem, IAreaOfEffectAbility
    {
        protected AreaOfEffectItem(Item item)
            : base(item)
        {
        }

        public virtual float Radius
        {
            get
            {
                return GetAbilitySpecialData("radius");
            }
        }

        public override bool CanHit(Unit target)
        {
            return Owner.Distance2D(target) < CastRange + Radius;
        }
    }
}