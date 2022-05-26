using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BAIO.Heroes.Base;
using BAIO.Modes;
using Ensage;
using log4net;
using PlaySharp.Toolkit.Logging;

namespace BAIO.Heroes.Modes.Harass
{
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;

    class HuskarHarass : HarassMode
    {
        private readonly Huskar Huskar;
        private Hero HarassTarget;
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HuskarHarass(Huskar hero) : base(hero)
        {
            this.Huskar = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var target = this.GetTarget();

            if (target != null)
            {
                this.Orbwalker.OrbwalkTo(target);
                return;
            }

            if (Huskar.FireSpear.Enabled)
            {
                this.Huskar.FireSpear.Enabled = false;
            }

            HarassTarget =
                EntityManager<Hero>.Entities.Where(
                        x => x.IsValid && !x.IsMagicImmune() &&
                            x.IsVisible && x.IsAlive && !x.IsIllusion && x.Team != this.Owner.Team &&
                            this.Owner.IsInAttackRange(x, this.BonusAttackRange))
                    .OrderBy(x => x.Health).FirstOrDefault();

            if (HarassTarget != null && this.Owner.CanAttack())
            {
                var useSpears = Huskar.IsFireSpearGucci(HarassTarget);

                if (useSpears)
                {
                    await this.Huskar.UseFireSpear(HarassTarget, token);
                }

                this.Orbwalker.OrbwalkTo(HarassTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
        }
    }
}
