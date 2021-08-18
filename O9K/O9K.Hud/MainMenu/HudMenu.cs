﻿namespace O9K.Hud.MainMenu
{
    using Core.Managers.Context;
    using Core.Managers.Menu;
    using Core.Managers.Menu.Items;

    using Divine.Renderer;

    using Modules;

    internal class HudMenu : IHudModule, IHudMenu
    {
        public HudMenu()
        {
            this.RootMenu = new Menu("Hud", "O9K.Hud").SetTexture("o9k.me");

            this.UnitsMenu = this.RootMenu.Add(new Menu("Units"));
            this.UnitsMenu.AddTranslation(Lang.Ru, "Юниты");
            this.UnitsMenu.AddTranslation(Lang.Cn, "单位");

            this.TopPanelMenu = this.RootMenu.Add(new Menu("Top panel"));
            this.TopPanelMenu.AddTranslation(Lang.Ru, "Верхняя панель");
            this.TopPanelMenu.AddTranslation(Lang.Cn, "顶部面板");

            this.TopPanelSettingsMenu = this.TopPanelMenu.Add(new Menu("Settings"));
            this.TopPanelSettingsMenu.AddTranslation(Lang.Ru, "Настройки");
            this.TopPanelSettingsMenu.AddTranslation(Lang.Cn, "设置");

            this.MapMenu = this.RootMenu.Add(new Menu("Map"));
            this.MapMenu.AddTranslation(Lang.Ru, "Карта");
            this.MapMenu.AddTranslation(Lang.Cn, "地图");

            this.MinimapSettingsMenu = this.MapMenu.Add(new Menu("Minimap settings"));
            this.MinimapSettingsMenu.AddTranslation(Lang.Ru, "Настройки миникарты");
            this.MinimapSettingsMenu.AddTranslation(Lang.Cn, "小地图状态");

            this.ParticlesMenu = this.RootMenu.Add(new Menu("Particles"));
            this.ParticlesMenu.AddTranslation(Lang.Ru, "Партиклы");
            this.ParticlesMenu.AddTranslation(Lang.Cn, "粒子效果");

            this.NotificationsMenu = this.RootMenu.Add(new Menu("Notifications"));
            this.NotificationsMenu.AddTranslation(Lang.Ru, "Оповещения");
            this.NotificationsMenu.AddTranslation(Lang.Cn, "通知提示");

            this.NotificationsSettingsMenu = this.NotificationsMenu.Add(new Menu("Settings"));
            this.NotificationsSettingsMenu.AddTranslation(Lang.Ru, "Настройки");
            this.NotificationsSettingsMenu.AddTranslation(Lang.Cn, "设置");

            this.ScreenMenu = this.RootMenu.Add(new Menu("Screen"));
            this.ScreenMenu.AddTranslation(Lang.Ru, "Экран");
            this.ScreenMenu.AddTranslation(Lang.Cn, "屏幕");

            this.UniqueMenu = this.RootMenu.Add(new Menu("Unique"));
            this.UniqueMenu.AddTranslation(Lang.Ru, "Уникальные");
            this.UniqueMenu.AddTranslation(Lang.Cn, "独特");

            this.DontDrawWhenShopIsOpen = this.RootMenu.Add(new MenuSwitcher("Dont draw when shop is open", false));
            this.DontDrawWhenShopIsOpen.AddTranslation(Lang.Ru, "Не рисовать, когда открыт магазин");
            this.DontDrawWhenShopIsOpen.AddTranslation(Lang.Cn, "商店营业时不要画画");
        }

        public MenuSwitcher DontDrawWhenShopIsOpen { get; set; }

        public Menu MapMenu { get; }

        public Menu MinimapSettingsMenu { get; }

        public Menu NotificationsMenu { get; }

        public Menu NotificationsSettingsMenu { get; }

        public Menu ParticlesMenu { get; }

        public Menu RootMenu { get; }

        public Menu ScreenMenu { get; }

        public Menu TopPanelMenu { get; }

        public Menu TopPanelSettingsMenu { get; }

        public Menu UniqueMenu { get; }

        public Menu UnitsMenu { get; }

        public void Activate()
        {
            RendererManager.LoadImage("o9k.me", @"panorama\images\textures\minimap_hero_self_psd.vtex_c");
            Context9.MenuManager.AddRootMenu(this.RootMenu);
        }

        public void Dispose()
        {
            Context9.MenuManager.RemoveRootMenu(this.RootMenu);
        }
    }
}