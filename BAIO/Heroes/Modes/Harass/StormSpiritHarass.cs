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

    class StormSpiritHarass : HarassMode
    {
        private readonly StormSpirit StormSpirit;
        private Hero HarassTarget;
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public StormSpiritHarass(StormSpirit hero) : base(hero)
        {
            this.StormSpirit = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var target = this.GetTarget();

            if (target != null)
            {
                this.Orbwalker.OrbwalkTo(target);
                return;
            }

            HarassTarget =
                EntityManager<Hero>.Entities.Where(
                        x => x.IsValid && !x.IsMagicImmune() &&
                            x.IsVisible && x.IsAlive && !x.IsIllusion && x.Team != this.Owner.Team &&
                            this.Owner.IsInAttackRange(x, this.BonusAttackRange))
                    .OrderBy(x => x.Health).FirstOrDefault();

            if (HarassTarget != null && this.Owner.CanAttack())
            {
                var ultiPos = this.StormSpirit.UltiPos(this.HarassTarget);
                if (!this.StormSpirit.InOverload && this.StormSpirit.Ulti.CanBeCasted)
                {
                    this.StormSpirit.Ulti.UseAbility(ultiPos);
                    await Task.Delay(this.StormSpirit.Ulti.GetCastDelay() + 500, token);
                }

                this.Orbwalker.OrbwalkTo(HarassTarget);
                return;
            }

            this.Orbwalker.OrbwalkTo(null);
        }
    }
}
