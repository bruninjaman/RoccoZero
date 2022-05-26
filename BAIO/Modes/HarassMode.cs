namespace BAIO.Modes
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Threading;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Orbwalker.Modes;

    public class HarassMode : AttackOrbwalkingModeAsync
    {
        private readonly BaseHero BaseHero;

        public HarassMode(BaseHero hero)
            : base(hero.Context, "BAIO Harass", 88, false, false, false, false, true, true)
        {
            this.BaseHero = hero;
        }

        protected float BonusAttackRange => this.Selector?.BonusRange ?? 0.0f;

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var unitTarget = this.GetTarget();
            if (unitTarget != null)
            {
                this.Orbwalker.OrbwalkTo(unitTarget);
                return;
            }

            var harrasTarget = EntityManager<Hero>.Entities.Where(x => x.IsVisible && x.IsAlive && !x.IsIllusion && x.IsEnemy(this.Owner) && this.Owner.IsInAttackRange(x, this.BonusAttackRange))
                                                  .OrderBy(x => x.Health)
                                                  .FirstOrDefault();
            if (harrasTarget != null)
            {
                this.Orbwalker.OrbwalkTo(harrasTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
            await Await.Delay(10, token);
        }
    }
}