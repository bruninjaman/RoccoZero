using Divine.SDK.Extensions;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class AreaOfEffectSpell : RangedSpell, IAreaOfEffectAbility
    {
        protected AreaOfEffectSpell(Ability ability)
            : base(ability)
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