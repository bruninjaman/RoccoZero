namespace O9K.AIO.Heroes.ArcWarden
{
    using System.Collections.Generic;

    using AIO.Modes.Combo;
    using AIO.Modes.KeyPress;

    using Base;

    using Core.Entities.Metadata;
    using Core.Managers.Menu.Items;

    using CustomUnitManager;

    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Input;
    using Divine.Renderer;

    using Draw;

    using Modes;

    [HeroId(HeroId.npc_dota_hero_arc_warden)]
    internal class ArcWardenBase : BaseHero
    {
        private readonly PushMode pushMode;

        public ArcWardenBase()
        {
            this.pushMode = new PushMode(this, new KeyPressModeMenu(this.Menu.RootMenu, "Push mode"));
            RendererManager.Draw += ArcWardenDrawPanel.ButtonDrawOn;
            InputManager.MouseKeyDown += ArcWardenDrawPanel.OnMouseKeyDown;
            var menuPanelSetting = Menu.RootMenu.Add(new Menu("Panel settings"));

            menuPanelSetting.Add(ArcWardenDrawPanel.positionSliderX);
            menuPanelSetting.Add(ArcWardenDrawPanel.positionSliderY);
            menuPanelSetting.Add(ArcWardenDrawPanel.sizeMenuSlider);
        }

        public override void CreateUnitManager()
        {
            this.UnitManager = new ArcWardenUnitManager(this);
        }

        public override void CreateComboMode(BaseHero baseHero, List<ComboModeMenu> comboMenus)
        {
            this.Combo = new ArcWardenComboMode(this, comboMenus);
        }

        protected override void CreateComboMenus()
        {
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Combo"));
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Alternative combo"));
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Clone combo"));
        }

        public override void Dispose()
        {
            base.Dispose();
            this.pushMode.Dispose();
        }

        protected override void DisableCustomModes()
        {
            this.pushMode.Disable();
        }

        protected override void EnableCustomModes()
        {
            this.pushMode.Enable();
        }
    }
}