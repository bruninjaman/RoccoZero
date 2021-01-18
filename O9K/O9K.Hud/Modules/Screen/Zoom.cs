namespace O9K.Hud.Modules.Screen
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using Core.Data;
    using Core.Helpers;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine;

    using MainMenu;

    using KeyEventArgs = Core.Managers.Menu.EventArgs.KeyEventArgs;

    internal class Zoom : IHudModule
    {
        private readonly Dictionary<string, int> consoleCommands = new Dictionary<string, int>
        {
            { "r_farz", 10000 },
            { "fog_enable", 0 },
        };

        private readonly MenuSwitcher enabled;

        private readonly MenuHoldKey key;

        private readonly MenuSlider zoom;

        public Zoom(IHudMenu hudMenu)
        {
            var menu = hudMenu.ScreenMenu.GetOrAdd(new Menu("Zoom"));
            menu.AddTranslation(Lang.Ru, "Зумхак");
            menu.AddTranslation(Lang.Cn, "视野");

            this.enabled = menu.Add(new MenuSwitcher("Enabled", false));
            this.enabled.AddTranslation(Lang.Ru, "Включено");
            this.enabled.AddTranslation(Lang.Cn, "启用");

            this.zoom = menu.Add(new MenuSlider("Zoom", 1400, GameData.DefaultZoom, 3000)).SetTooltip("Default: " + GameData.DefaultZoom);
            this.zoom.AddTranslation(Lang.Ru, "Зум");
            this.zoom.AddTooltipTranslation(Lang.Ru, "По умолчанию: " + GameData.DefaultZoom);
            this.zoom.AddTranslation(Lang.Cn, "视野");
            this.zoom.AddTooltipTranslation(Lang.Cn, "默认值：" + GameData.DefaultZoom);

            this.key = menu.Add(new MenuHoldKey("Key", Key.LeftCtrl)).SetTooltip("Change zoom with a key and mouse wheel");
            this.key.AddTranslation(Lang.Ru, "Клавиша");
            this.key.AddTooltipTranslation(Lang.Ru, "Изменить зум с помощью клавиши и колесика мыши");
            this.key.AddTranslation(Lang.Cn, "键");
            this.key.AddTooltipTranslation(Lang.Cn, "使用键和鼠标滚轮更改缩放");
        }

        public void Activate()
        {
            this.enabled.ValueChange += this.EnabledOnValueChange;
        }

        public void Dispose()
        {
            this.enabled.ValueChange -= this.EnabledOnValueChange;
            this.key.ValueChange -= this.KeyOnValueChange;
            this.zoom.ValueChange -= this.ZoomOnValueChange;
            InputManager.MouseWheel -= this.OnMouseWheel;
        }

        private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                if (e.OldValue)
                {
                    //todo delete
                    if (AppDomain.CurrentDomain.GetAssemblies().Any(x => !x.GlobalAssemblyCache && x.GetName().Name.Contains("Zoomhack")))
                    {
                        Hud.DisplayWarning("O9K.Hud // ZoomHack is already included in O9K.Hud");
                    }
                }

                foreach (var cmd in this.consoleCommands)
                {
                    ConVarManager.SetValue(cmd.Key, cmd.Value);
                }

                this.zoom.ValueChange += this.ZoomOnValueChange;
                this.key.ValueChange += this.KeyOnValueChange;
            }
            else
            {
                this.key.ValueChange -= this.KeyOnValueChange;
                this.zoom.ValueChange -= this.ZoomOnValueChange;
                InputManager.MouseWheel -= this.OnMouseWheel;

                ConVarManager.SetValue("dota_camera_distance", GameData.DefaultZoom);
            }
        }

        private void KeyOnValueChange(object sender, KeyEventArgs e)
        {
            if (e.NewValue)
            {
                InputManager.MouseWheel += this.OnMouseWheel;
            }
            else
            {
                InputManager.MouseWheel -= this.OnMouseWheel;
            }
        }

        private void OnMouseWheel(MouseWheelEventArgs e)
        {
            this.zoom.Value += e.Up ? -50 : 50;
            e.Process = false;
        }

        private void ZoomOnValueChange(object sender, SliderEventArgs e)
        {
            ConVarManager.SetValue("dota_camera_distance", e.NewValue);
        }
    }
}