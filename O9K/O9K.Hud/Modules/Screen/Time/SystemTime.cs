﻿namespace O9K.Hud.Modules.Screen.Time;

using System;
using System.Globalization;

using Core.Helpers;
using Core.Logger;
using Core.Managers.Menu;
using Core.Managers.Menu.EventArgs;
using Core.Managers.Menu.Items;

using Divine.Game;
using Divine.Numerics;
using Divine.Renderer;

using Helpers;

using MainMenu;

internal class SystemTime : IHudModule
{
    private readonly MenuSwitcher enabled;

    private readonly MenuVectorSlider position;

    private readonly MenuSlider textSize;

    private readonly string timeFormat;

    private readonly IHudMenu hudMenu;

    public SystemTime(IHudMenu hudMenu)
    {
        this.hudMenu = hudMenu;

        var timeMenu = hudMenu.ScreenMenu.GetOrAdd(new Menu("Time"));
        timeMenu.AddTranslation(Lang.Ru, "Время");
        timeMenu.AddTranslation(Lang.Cn, "时间");

        var menu = timeMenu.Add(new Menu("System time"));
        menu.AddTranslation(Lang.Ru, "Системное время");
        menu.AddTranslation(Lang.Cn, "系统时间");

        this.enabled = menu.Add(new MenuSwitcher("Enabled", false).SetTooltip("Show your pc's time"));
        this.enabled.AddTranslation(Lang.Ru, "Включено");
        this.enabled.AddTooltipTranslation(Lang.Ru, "Показывать время пк");
        this.enabled.AddTranslation(Lang.Cn, "启用");
        this.enabled.AddTooltipTranslation(Lang.Cn, "显示您的电脑时间");

        var settings = menu.Add(new Menu("Settings"));
        settings.AddTranslation(Lang.Ru, "Настройки");
        settings.AddTranslation(Lang.Cn, "设置");

        this.textSize = settings.Add(new MenuSlider("Size", 16, 10, 35));
        this.textSize.AddTranslation(Lang.Ru, "Размер");
        this.textSize.AddTranslation(Lang.Cn, "大小");

        this.position = new MenuVectorSlider(settings, new Vector2(Hud.Info.ScreenSize.X, Hud.Info.ScreenSize.Y * 0.0408f));

        this.timeFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
    }

    public void Activate()
    {
        RendererManager.LoadImage(
            "o9k.time_bg",
            @"panorama\images\masks\gradient_rightleft_png.vtex_c",
            new ImageProperties
            {
                Brightness = -180,
                ColorTint = new Color(0, 0, 0, 204)
            });

        this.enabled.ValueChange += this.EnabledOnValueChange;
    }

    public void Dispose()
    {
        this.position.Dispose();
        this.enabled.ValueChange -= this.EnabledOnValueChange;
        RendererManager.Draw -= this.OnDraw;
    }

    private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            RendererManager.Draw += this.OnDraw;
        }
        else
        {
            RendererManager.Draw -= this.OnDraw;
        }
    }

    private void OnDraw()
    {
        if (GameManager.IsShopOpen && this.hudMenu.DontDrawWhenShopIsOpen)
        {
            return;
        }

        try
        {
            var time = DateTime.Now.ToString(this.timeFormat);
            var timeSize = RendererManager.MeasureText(time, this.textSize);

            var bgWidth = timeSize.X * 2.5f;
            var bgPosition = this.position - new Vector2(bgWidth, 0);
            var textPosition = this.position - new Vector2(timeSize.X + (4 * Hud.Info.ScreenRatio), 0);

            RendererManager.DrawImage("o9k.time_bg", new RectangleF(bgPosition.X, bgPosition.Y, bgWidth, this.textSize * 1.25f));
            RendererManager.DrawText(time, textPosition, Color.LightGray, this.textSize);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}