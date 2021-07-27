namespace O9K.AIO.Heroes.ArcWarden
{
    using System.Collections.Generic;
    using Base;
    using Core.Entities.Metadata;
    using CustomUnitManager;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Modes.Combo;

    [HeroId(HeroId.npc_dota_hero_arc_warden)]
    internal class ArcWardenBase : BaseHero
    {
        public ArcWardenBase()
        {
        }

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