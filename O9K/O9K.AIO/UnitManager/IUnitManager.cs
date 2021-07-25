using System;
using System.Collections.Generic;
using Divine.Projectile.EventArgs;
using O9K.AIO.Heroes.Base;
using O9K.AIO.Modes.Combo;
using O9K.AIO.Modes.MoveCombo;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Units;
using O9K.Core.Managers.Menu.EventArgs;

namespace O9K.AIO.UnitManager
{
    internal interface IUnitManager : IDisposable
    {
        IEnumerable<ControllableUnit> ControllableUnits { get; }
        IEnumerable<ControllableUnit> AllControllableUnits { get; }
        BaseHero BaseHero { get; }
        Core.Managers.Menu.Items.Menu Menu { get; }
        void Disable();
        void Dispose();
        void Enable();
        void EndCombo(ComboModeMenu comboModeMenu);
        void ExecuteCombo(ComboModeMenu comboModeMenu);
        void ExecuteMoveCombo(MoveComboModeMenu comboModeMenu);
        void Move();
        void Orbwalk(ControllableUnit controllable, bool attack, bool move);
        void Orbwalk(ComboModeMenu comboModeMenu);
        bool BodyBlock(ICollection<ControllableUnit> allUnits, ComboModeMenu comboModeMenu);
        void ControlAllUnits(IEnumerable<ControllableUnit> noOrbwalkUnits);
        ControllableUnitMenu GetUnitMenu(Unit9 unit);
        float IssuedActionTime(uint handle);
        void OnAbilityAdded(Ability9 entity);
        void OnAbilityRemoved(Ability9 entity);
        void OnInventoryChanged(object sender, EventArgs e);
        void ControlAlliesOnValueChanged(object sender, SwitcherEventArgs e);
        void OnAddTrackingProjectile(TrackingProjectileAddedEventArgs e);
        void OnAttackStart(Unit9 unit);
        void OnUnitAdded(Unit9 entity);
        void OnUnitRemoved(Unit9 entity);
    }
}