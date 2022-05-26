namespace BAIO.Heroes.Modes.Harass
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Ensage.SDK.Extensions;

    public class DrowHarass : BAIO.Modes.HarassMode
    {
        private readonly BAIO.Heroes.Base.DrowRanger DrowRanger;

        public DrowHarass(BAIO.Heroes.Base.DrowRanger hero)
            : base(hero)
        {
            this.DrowRanger = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var unitTarget = this.GetTarget();
            if (unitTarget != null)
            {
                this.Orbwalker.OrbwalkTo(unitTarget);
                return;
            }

            var harrasTarget = Ensage.SDK.Helpers.EntityManager<Ensage.Hero>
                .Entities.Where(x => x.IsVisible && x.IsAlive && !x.IsIllusion && x.IsEnemy(this.Owner) && this.Owner.IsInAttackRange(x, this.BonusAttackRange))
                .OrderBy(x => x.Health)
                .FirstOrDefault();

            if (harrasTarget != null)
            {
                var useArrow = this.DrowRanger.ShouldUseFrostArrow(harrasTarget);
                if (useArrow)
                {
                    await this.DrowRanger.UseFrostArrows(harrasTarget, token);
                    return;
                }

                this.Orbwalker.OrbwalkTo(harrasTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
        }
    }
}
