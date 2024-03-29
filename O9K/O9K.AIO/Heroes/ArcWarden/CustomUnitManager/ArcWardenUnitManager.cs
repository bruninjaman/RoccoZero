﻿namespace O9K.AIO.Heroes.ArcWarden.CustomUnitManager
{
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.Combo;

    using Base;

    using Divine.Game;
    using Divine.Helpers;
    using Divine.Numerics;

    using UnitManager;

    internal class ArcWardenUnitManager : UnitManager
    {
        public ArcWardenUnitManager(BaseHero baseHero)
            : base(baseHero)
        {
        }

        private Sleeper unitSwitchSleeper = new();
        private ControllableUnit lastUnit;

        public IEnumerable<ControllableUnit> CloneControllableUnits
        {
            get
            {
                return this.controllableUnits.Where(
                    x => x.IsValid && x.Owner.IsIllusion && x.CanBeControlled &&
                         x.Owner.Distance(this.targetManager.Target ?? this.owner) < 2500);
            }
        }

        public IEnumerable<ControllableUnit> PushControllableUnits
        {
            get
            {
                return this.controllableUnits.Where(
                    x => x.IsValid && x.Owner.IsIllusion && x.CanBeControlled && x.ShouldControl);
            }
        }

        public virtual void ExecuteCloneCombo(ComboModeMenu comboModeMenu)
        {
            foreach (var controllable in this.CloneControllableUnits)
            {
                if (controllable.ComboSleeper.IsSleeping || (unitSwitchSleeper.Sleeping && lastUnit != controllable))
                {
                    continue;
                }

                if (!comboModeMenu.IgnoreInvisibility && controllable.IsInvisible && (!comboModeMenu.IgnoreInvisibilityIfVisible || !controllable.Owner.IsVisibleToEnemies))
                {
                    continue;
                }

                if (controllable.Combo(this.targetManager, comboModeMenu))
                {
                    controllable.LastMovePosition = Vector3.Zero;
                    lastUnit = controllable;
                    unitSwitchSleeper.Sleep(200);
                }
            }
        }

        public virtual void CloneOrbwalk(ComboModeMenu comboModeMenu)
        {
            if (this.issuedAction.IsSleeping)
            {
                return;
            }

            var allUnits = this.CloneControllableUnits.OrderBy(x => this.IssuedActionTime(x.Handle)).ToList();

            if (this.BodyBlock(allUnits, comboModeMenu))
            {
                this.issuedAction.Sleep(0.05f);

                return;
            }

            var noOrbwalkUnits = new List<ControllableUnit>();

            foreach (var controllable in allUnits)
            {
                if (!controllable.OrbwalkEnabled)
                {
                    noOrbwalkUnits.Add(controllable);

                    continue;
                }

                if (this.unitIssuedAction.IsSleeping(controllable.Handle))
                {
                    continue;
                }

                if (!controllable.Orbwalk(this.targetManager.Target, comboModeMenu))
                {
                    continue;
                }

                this.issuedActionTimings[controllable.Handle] = GameManager.RawGameTime;
                this.unitIssuedAction.Sleep(controllable.Handle, 0.2f);
                this.issuedAction.Sleep(0.05f);

                return;
            }

            if (noOrbwalkUnits.Count > 0 && !this.unitIssuedAction.IsSleeping(uint.MaxValue))
            {
                this.ControlAllUnits(noOrbwalkUnits);
            }
        }
    }
}