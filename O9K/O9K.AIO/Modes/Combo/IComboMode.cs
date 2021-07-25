using System.Collections.Generic;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Order.EventArgs;
using O9K.AIO.Heroes.Base;
using O9K.AIO.Menu;
using O9K.AIO.UnitManager;
using O9K.Core.Entities.Abilities.Base.Components;
using O9K.Core.Entities.Heroes;
using O9K.Core.Managers.Menu.EventArgs;
using O9K.Core.Managers.Menu.Items;

namespace O9K.AIO.Modes.Combo
{
    internal interface IComboMode
    {
        Dictionary<MenuHoldKey, ComboModeMenu> ComboModeMenus { get; }
        List<uint> DisableToggleAbilities { get; }
        HashSet<AbilityId> IgnoreToggleDisable { get; }
        ComboModeMenu ComboModeMenu { get; }
        IUnitManager UnitManager { get; }
        BaseHero BaseHero { get; }
        MenuManager Menu { get; }
        Owner Owner { get; }
        TargetManager.TargetManager TargetManager { get; }
        void Disable();
        void Dispose();
        void Enable();
        void ComboEnd();
        void OnUpdate();
        void KeyOnValueChanged(object sender, KeyEventArgs e);
        void OnOrderAdding(OrderAddingEventArgs e);
        void ToggleAbility(IToggleable toggle);
    }
}