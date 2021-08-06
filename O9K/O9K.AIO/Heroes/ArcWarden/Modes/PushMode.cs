namespace O9K.AIO.Heroes.ArcWarden.Modes
{
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.KeyPress;

    using Base;

    using Core.Managers.Menu.EventArgs;

    using CustomUnitManager;

    using Draw;

    using Units;

    internal class PushMode : KeyPressMode
    {
        private ArcWarden hero;

        private ArcWardenUnitManager arcUnitManager1;

        public PushMode(BaseHero baseHero, KeyPressModeMenu menu)
            : base(baseHero, menu)
        {
            arcUnitManager1 = UnitManager as ArcWardenUnitManager;
        }

        private ArcWarden Hero
        {
            get
            {
                if (this.hero == null)
                {
                    this.hero = arcUnitManager1.PushControllableUnits.Where(
                                    x => x.Owner.IsHero).FirstOrDefault() as ArcWarden;
                }

                return this.hero;
            }
        }

        protected override void KeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (!e.NewValue)
            {
                return;
            }

            if (!this.UpdateHandler.IsEnabled)
            {
                this.UpdateHandler.IsEnabled = true;

                AutoPushingPanelTest.pushComboStatus = true;
            }
            else
            {
                this.UpdateHandler.IsEnabled = false;

                AutoPushingPanelTest.pushComboStatus = false;
            }
        }

        public IEnumerable<ArcWarden> ControllableUnitsTempest
        {
            get
            {
                return arcUnitManager1.PushControllableUnits.Where(
                    x => x.Owner.IsIllusion && x.Owner.Distance(this.Hero.Owner) < 1000).Cast<ArcWarden>();
            }
        }

        protected override void ExecuteCombo()
        {
            if (this.Hero == null)
            {
                return;
            }

            if (ControllableUnitsTempest.Any(x => x.IsValid))
            {
                foreach (var unit in ControllableUnitsTempest)
                {
                    unit.PushCombo();
                }
            }
        }
    }
}