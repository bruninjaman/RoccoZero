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
        private readonly ArcWardenUnitManager arcUnitManager1;

        private ArcWarden hero;

        public PushMode(BaseHero baseHero, KeyPressModeMenu menu)
            : base(baseHero, menu)
        {
            Instance = this;
            arcUnitManager1 = UnitManager as ArcWardenUnitManager;
        }

        private ArcWarden Hero
        {
            get
            {
                if (hero == null)
                {
                    hero = arcUnitManager1.PushControllableUnits.Where(
                               x => x.Owner.IsHero).FirstOrDefault() as ArcWarden;
                }

                return hero;
            }
        }

        public static PushMode Instance { get; set; }

        public IEnumerable<IPushUnit> ControllableUnitsTempest
        {
            get
            {
                return  arcUnitManager1.PushControllableUnits.Where(
                        x => x.Owner.IsIllusion && x.Owner != Hero?.Owner)
                    .Select(x => new PushUnit(x) as IPushUnit);
            }
        }

        public void TurnOffAutoPush()
        {
            UpdateHandler.IsEnabled = false;

            ArcWardenDrawPanel.pushComboStatus = false;
        }

        protected override void KeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (!e.NewValue)
            {
                return;
            }

            if (!UpdateHandler.IsEnabled)
            {
                ArcWarden.TpCount = 1;

                UpdateHandler.IsEnabled = true;

                ArcWardenDrawPanel.pushComboStatus = true;
            }
            else
            {
                UpdateHandler.IsEnabled = false;

                ArcWardenDrawPanel.pushComboStatus = false;
            }
        }

        protected override void ExecuteCombo()
        {
            // if (this.Hero != null)
            // {
            //     var pushUnits = ControllableUnitsTempest.Append(Hero);
            // }
            //
            // if (this.Hero.IsValid)
            // {
            //     this.Hero.PushCombo();
            // }

            var controllableUnitsTempest = ControllableUnitsTempest;

            if (Hero != null)
            {
                controllableUnitsTempest = controllableUnitsTempest.Append(Hero);
            }

            if (!controllableUnitsTempest.Any(x => x != null && x.IsValid))
            {
                UpdateHandler.IsEnabled = false;

                ArcWardenDrawPanel.pushComboStatus = false;
            }

            foreach (var unit in controllableUnitsTempest.Where(x => x != null && x.IsValid))
            {
                unit.PushCombo();
            }
        }
    }
}