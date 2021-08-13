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
            this.arcUnitManager1 = this.UnitManager as ArcWardenUnitManager;
        }

        private ArcWarden Hero
        {
            get
            {
                if (this.hero == null)
                {
                    this.hero = this.arcUnitManager1.PushControllableUnits.Where(
                                    x => x.Owner.IsHero).FirstOrDefault() as ArcWarden;
                }

                return this.hero;
            }
        }

        public static PushMode Instance { get; set; }

        public IEnumerable<IPushUnit> ControllableUnitsTempest
        {
            get
            {
                return  this.arcUnitManager1.PushControllableUnits.Where(
                        x => x.Owner.IsIllusion && x.Owner != this.Hero?.Owner)
                    .Select(x => new PushUnit(x) as IPushUnit);
            }
        }

        public void TurnOffAutoPush()
        {
            this.UpdateHandler.IsEnabled = false;

            ArcWardenDrawPanel.pushComboStatus = false;
        }

        protected override void KeyOnValueChanged(object sender, KeyEventArgs e)
        {
            if (!e.NewValue)
            {
                return;
            }

            if (!this.UpdateHandler.IsEnabled)
            {
                ArcWarden.TpCount = 1;

                this.UpdateHandler.IsEnabled = true;

                ArcWardenDrawPanel.pushComboStatus = true;
            }
            else
            {
                this.UpdateHandler.IsEnabled = false;

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

            var controllableUnitsTempest = this.ControllableUnitsTempest;

            if (this.Hero != null)
            {
                controllableUnitsTempest = controllableUnitsTempest.Append(this.Hero);
            }

            if (!controllableUnitsTempest.Any(x => x != null && x.IsValid))
            {
                this.UpdateHandler.IsEnabled = false;

                ArcWardenDrawPanel.pushComboStatus = false;
            }

            foreach (var unit in controllableUnitsTempest.Where(x => x != null && x.IsValid))
            {
                unit.PushCombo();
            }
        }
    }
}