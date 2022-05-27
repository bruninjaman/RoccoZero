namespace Ensage.SDK.Orbwalker.Modes
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Divine.Entity.Entities.Units;

    using Ensage.SDK.Service;
    using Ensage.SDK.TargetSelector;

    public abstract class AutoAttackModeAsync : OrbwalkingModeAsync
    {
        protected AutoAttackModeAsync(IServiceContext context)
            : base(context)
        {
            this.TargetSelector = context.TargetSelector;
        }

        protected ITargetSelectorManager TargetSelector { get; }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            this.Orbwalker.OrbwalkTo(this.GetTarget());
        }

        protected virtual Unit GetTarget()
        {
            return this.TargetSelector.GetTargets().FirstOrDefault();
        }
    }
}