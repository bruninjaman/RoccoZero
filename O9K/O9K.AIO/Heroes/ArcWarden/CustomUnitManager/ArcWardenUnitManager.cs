namespace O9K.AIO.Heroes.ArcWarden.CustomUnitManager
{
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.Combo;

    using Base;

    using Core.Entities.Units;
    using Core.Managers.Entity;

    using Divine.Game;
    using Divine.Numerics;

    using TargetManager;

    using UnitManager;

    internal sealed class ArcWardenUnitManager : UnitManager
    {
        private readonly TargetManager cloneTargetManager;

        private TargetManager GetTargetManagerForClone
        {
            get
            {
                return this.cloneTargetManager.Target.IsAlive
                       && this.cloneTargetManager.Target.IsValid
                       && this.GetClone.Owner.Distance(this.cloneTargetManager.Target) < 2000
                       && this.cloneTargetManager.Target != EntityManager9.Owner
                           ? this.cloneTargetManager
                           : this.targetManager;
            }
        }

        public ArcWardenUnitManager(BaseHero baseHero)
            : base(baseHero)
        {
            this.cloneTargetManager = new TargetManager(baseHero.Menu);
        }

        public void SetTargetForClone(Unit9 unit)
        {
            this.cloneTargetManager.ForceSetTarget(unit);
        }

        public ControllableUnit GetClone
        {
            get
            {
                return this.CloneControllableUnits.FirstOrDefault(x => x.Owner.IsHero && x.Owner.IsIllusion);
            }
        }

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

        public void ExecuteCloneCombo(ComboModeMenu comboModeMenu)
        {

            foreach (var controllable in this.CloneControllableUnits)
            {
                if (controllable.ComboSleeper.IsSleeping)
                {
                    continue;
                }

                if (!comboModeMenu.IgnoreInvisibility && controllable.IsInvisible)
                {
                    return;
                }

                if (controllable.Combo(this.GetTargetManagerForClone, comboModeMenu))
                {
                    controllable.LastMovePosition = Vector3.Zero;
                }
            }
        }

        public void CloneOrbwalk(ComboModeMenu comboModeMenu)
        {

            if (this.issuedAction.IsSleeping)
            {
                return;
            }

            var allUnits = this.CloneControllableUnits.OrderBy(x => IssuedActionTime(x.Handle)).ToList();

            if (BodyBlock(allUnits, comboModeMenu))
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

                if (!controllable.Orbwalk(this.GetTargetManagerForClone.Target, comboModeMenu))
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
                ControlAllUnits(noOrbwalkUnits);
            }
        }
    }
}