using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Input;
using Divine.Menu;
using O9K.AIO.Heroes.ArcWarden.CustomUnitManager;
using O9K.AIO.Modes.Combo;
using O9K.AIO.Modes.KeyPress;

namespace O9K.AIO.Heroes.ArcWarden
{
    using Base;
    using Core.Entities.Metadata;
    using Divine.Entity.Entities.Units.Heroes.Components;

    [HeroId(HeroId.npc_dota_hero_arc_warden)]
    internal class ArcWardenBase : BaseHero
    {
        public ArcWardenBase()
        {
        }

        // protected override void DisableCustomModes()
        // {
        //     this.Combo.Disable();
        // }
        //
        // protected override void EnableCustomModes()
        // {
        //     this.Combo.Dispose();
        //     this.Combo.Enable();
        //
        // }

        public override void CreateUnitManager()
        {
            this.UnitManager = new ArcWardenUnitManager(this);
        }

        public override void CreateComboMode(BaseHero baseHero, List<ComboModeMenu> comboMenus)
        {
            this.Combo = new ArcWardenComboMode.ArcWardenComboMode(this, comboMenus);
        }

        protected override void CreateComboMenus()
        {
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Combo"));
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Alternative combo"));
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Clone combo"));
        }
    }
}