namespace O9K.AIO.Heroes.ArcWarden.CustomUnitManager
{
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.Combo;

    using Base;

    using Divine.Game;
    using Divine.Numerics;

    using UnitManager;

    internal class ArcWardenUnitManager : UnitManager
    {
        public ArcWardenUnitManager(BaseHero baseHero)
            : base(baseHero)
        {
        }

        public IEnumerable<ControllableUnit> CloneControllableUnits
        {
            get
            {
                return controllableUnits.Where(
                    x => x.IsValid && x.Owner.IsIllusion && x.CanBeControlled &&
                         x.Owner.Distance(targetManager.Target ?? owner) < 2500);
            }
        }

        public IEnumerable<ControllableUnit> PushControllableUnits
        {
            get
            {
                return controllableUnits.Where(
                    x => x.IsValid && x.Owner.IsIllusion && x.CanBeControlled && x.ShouldControl);
            }
        }

        public virtual void ExecuteCloneCombo(ComboModeMenu comboModeMenu)
        {
            foreach (var controllable in CloneControllableUnits)
            {
                if (controllable.ComboSleeper.IsSleeping)
                {
                    continue;
                }

                if (!comboModeMenu.IgnoreInvisibility && controllable.IsInvisible)
                {
                    return;
                }

                if (controllable.Combo(targetManager, comboModeMenu))
                {
                    controllable.LastMovePosition = Vector3.Zero;
                }
            }
        }

        public virtual void CloneOrbwalk(ComboModeMenu comboModeMenu)
        {
            if (issuedAction.IsSleeping)
            {
                return;
            }

            var allUnits = CloneControllableUnits.OrderBy(x => IssuedActionTime(x.Handle)).ToList();

            if (BodyBlock(allUnits, comboModeMenu))
            {
                issuedAction.Sleep(0.05f);

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

                if (unitIssuedAction.IsSleeping(controllable.Handle))
                {
                    continue;
                }

                if (!controllable.Orbwalk(targetManager.Target, comboModeMenu))
                {
                    continue;
                }

                issuedActionTimings[controllable.Handle] = GameManager.RawGameTime;
                unitIssuedAction.Sleep(controllable.Handle, 0.2f);
                issuedAction.Sleep(0.05f);

                return;
            }

            if (noOrbwalkUnits.Count > 0 && !unitIssuedAction.IsSleeping(uint.MaxValue))
            {
                ControlAllUnits(noOrbwalkUnits);
            }
        }
    }
}