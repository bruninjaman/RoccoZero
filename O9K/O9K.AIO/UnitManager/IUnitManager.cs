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
        void Enable();
        void EndCombo(ComboModeMenu comboModeMenu);
        void ExecuteCombo(ComboModeMenu comboModeMenu);
        void ExecuteMoveCombo(MoveComboModeMenu comboModeMenu);
        void Move();
        void Orbwalk(ControllableUnit controllable, bool attack, bool move);
        void Orbwalk(ComboModeMenu comboModeMenu);
    }
}