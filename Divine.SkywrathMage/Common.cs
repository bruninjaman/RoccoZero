using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Commons;
using Divine.Core.ComboFactory.Helpers;
using Divine.Core.ComboFactory.Menus;

using Divine.SkywrathMage.Combos;
using Divine.SkywrathMage.Features;
using Divine.SkywrathMage.Helpers;
using Divine.SkywrathMage.Menus;

namespace Divine.SkywrathMage
{
    internal sealed class Common : BaseCommon
    {
        public override BaseAbilities Abilities { get; } = new Abilities();

        public override BaseMenuConfig MenuConfig { get; } = new MenuConfig();

        public override BaseDamageCalculation DamageCalculation { get; }

        public override BaseLinkenBreaker LinkenBreaker { get; }

        public override BaseKillSteal KillSteal { get; }

        public override BaseCombo Combo { get; }

        public UpdateMode UpdateMode { get; }

        private AutoCombo AutoCombo { get; }

        private Farm Farm { get; }

        private AutoArcaneBolt AutoArcaneBolt { get; }

        private SpamArcaneBolt SpamArcaneBolt { get; }

        private Disable Disable { get; }

        private EulControl EulControl { get; }

        private ExecuteOrder ExecuteOrder { get; }

        public override BaseRenderer Renderer { get; set; }

        public Common()
        {
            DamageCalculation = new DamageCalculation(this);

            LinkenBreaker = new LinkenBreaker(this);
            KillSteal = new KillSteal(this);
            Combo = new Combo(this);

            UpdateMode = new UpdateMode(this);

            AutoCombo = new AutoCombo(this);
            Farm = new Farm(this);
            AutoArcaneBolt = new AutoArcaneBolt(this);
            SpamArcaneBolt = new SpamArcaneBolt(this);
            Disable = new Disable(this);
            EulControl = new EulControl(this);
            ExecuteOrder = new ExecuteOrder(this);

            Renderer = new Renderer(this);
        }

        public override void Dispose()
        {
            base.Dispose();

            ExecuteOrder.Dispose();
            EulControl.Dispose();
            Disable.Dispose();
            SpamArcaneBolt.Dispose();
            AutoArcaneBolt.Dispose();
            Farm.Dispose();
            AutoCombo.Dispose();

            UpdateMode.Dispose();
        }
    }
}
