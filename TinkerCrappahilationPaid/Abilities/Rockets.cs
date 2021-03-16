using System.Linq;
using Ensage;
using Ensage.SDK.Abilities;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;

namespace TinkerCrappahilationPaid.Abilities
{
    public class Rockets : RangedAbility
    {
        public Rockets(Ability ability) : base(ability)
        {
        }


        protected override float RawDamage
        {
            get
            {
                var damage = Ability.GetAbilitySpecialData("damage");
                return damage;
            }
        }

        public override float CastRange { get; } = 2500; 

        public override bool CanHit(params Unit[] targets)
        {
            var firstTarget = targets.First();
            var targetDist = firstTarget.Distance2D(Owner);
            var otherEnemies = EntityManager<Hero>.Entities.Where(x =>
                x.IsAlive && x.IsEnemy(Owner) && x.IsVisible && x.Distance2D(Owner) < targetDist && !firstTarget.Equals(x));
//            TinkerCrappahilationPaid.Log.Debug($"others: {otherEnemies.Count()} distToTarget: {targetDist} baseCanHit: ${base.CanHit(targets)} {base.CastRange} {base.BaseCastRange}");
            return base.CanHit(targets) && otherEnemies.Count() <= 1;
        }

        public bool CanHitAnyEnemy()
        {
            var illusions =
                EntityManager<Hero>.Entities.Where(x =>
                    x.IsAlive && x.IsVisible && x.IsEnemy(Owner) && x.IsIllusion && CanHit(x));
            var targets =
                EntityManager<Hero>.Entities.Where(x =>
                    x.IsAlive && x.IsVisible && x.IsEnemy(Owner) && !x.IsIllusion && CanHit(x));
            return targets.Any();
        }
    }
}