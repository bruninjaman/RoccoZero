﻿namespace O9K.Hud.Modules.Units.Information;

using System;
using System.Collections.Generic;

using Core.Entities.Heroes;
using Core.Entities.Units;
using Core.Logger;
using Core.Managers.Entity;
using Core.Managers.Menu;
using Core.Managers.Menu.EventArgs;
using Core.Managers.Menu.Items;

using Divine.Game;
using Divine.Input;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Modifier.Modifiers;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;

using Helpers;

using MainMenu;

internal class Information : IHudModule
{
    private readonly MenuSwitcher enabled;

    private readonly MenuSwitcher enabledDamage;

    private readonly MenuSwitcher enabledSpeed;

    private readonly MenuHoldKey holdKey;

    private readonly MenuVectorSlider position;

    private readonly MenuSlider size;

    private readonly MenuToggleKey toggleKey;

    private readonly List<InformationUnit> units = new List<InformationUnit>();

    private Owner owner;

    private readonly IHudMenu hudMenu;

    public Information(IHudMenu hudMenu)
    {
        this.hudMenu = hudMenu;

        var menu = hudMenu.UnitsMenu.GetOrAdd(new Menu("Information"));
        menu.AddTranslation(Lang.Ru, "Информация");
        menu.AddTranslation(Lang.Cn, "信息");

        this.enabled = menu.Add(new MenuSwitcher("Enabled").SetTooltip("Show additional enemy hero information"));
        this.enabled.AddTranslation(Lang.Ru, "Включено");
        this.enabled.AddTooltipTranslation(Lang.Ru, "Показывать дополнительную информацию о вражеских героях");
        this.enabled.AddTranslation(Lang.Cn, "启用");
        this.enabled.AddTooltipTranslation(Lang.Cn, "显示更多敌方英雄信息");

        this.enabledSpeed = menu.Add(new MenuSwitcher("Show speed").SetTooltip("Show speed difference"));
        this.enabledSpeed.AddTranslation(Lang.Ru, "Скорость");
        this.enabledSpeed.AddTooltipTranslation(Lang.Ru, "Показывать разницу в скорости");
        this.enabledSpeed.AddTranslation(Lang.Cn, "显示速度");
        this.enabledSpeed.AddTooltipTranslation(Lang.Cn, "显示速度差异");

        this.enabledDamage = menu.Add(new MenuSwitcher("Show damage").SetTooltip("Show required amount of auto attacks to kill"));
        this.enabledDamage.AddTranslation(Lang.Ru, "Урон");
        this.enabledDamage.AddTooltipTranslation(Lang.Ru, "Показывать необходимое количество авто атак для убийства");
        this.enabledDamage.AddTranslation(Lang.Cn, "显示伤害");
        this.enabledDamage.AddTooltipTranslation(Lang.Cn, "显示要杀死的自动攻击数量");

        var settings = menu.Add(new Menu("Settings"));
        settings.AddTranslation(Lang.Ru, "Настройки");
        settings.AddTranslation(Lang.Cn, "设置");

        this.position = new MenuVectorSlider(settings, new Vector3(12, -250, 250), new Vector3(70, -250, 250));
        this.size = settings.Add(new MenuSlider("Size", 18, 15, 25));
        this.size.AddTranslation(Lang.Ru, "Размер");
        this.size.AddTranslation(Lang.Cn, "大小");

        var keys = menu.Add(new Menu("Keys"));
        keys.AddTranslation(Lang.Ru, "Клавишы");
        keys.AddTranslation(Lang.Cn, "键");

        this.toggleKey = keys.Add(new MenuToggleKey("Toggle key", "toggle", Key.None, false)).SetTooltip("Show/hide information");
        this.toggleKey.AddTranslation(Lang.Ru, "Клавиша переключения");
        this.toggleKey.AddTooltipTranslation(Lang.Ru, "Показать/скрыть информацию");
        this.toggleKey.AddTranslation(Lang.Cn, "切换键");
        this.toggleKey.AddTooltipTranslation(Lang.Cn, "显示/隐藏信息");

        this.holdKey = keys.Add(new MenuHoldKey("Hold key", "hold", Key.LeftAlt)).SetTooltip("Show/hide information");
        this.holdKey.AddTranslation(Lang.Ru, "Клавиша удержания");
        this.holdKey.AddTooltipTranslation(Lang.Ru, "Показать/скрыть информацию");
        this.holdKey.AddTranslation(Lang.Cn, "按住键");
        this.holdKey.AddTooltipTranslation(Lang.Cn, "显示/隐藏信息");
    }

    public void Activate()
    {
        this.owner = EntityManager9.Owner;

        RendererManager.LoadImage(
            "o9k.attack_minimalistic",
            @"panorama\images\hud\reborn\icon_damage_psd.vtex_c");

        RendererManager.LoadImage(
            "o9k.speed_minimalistic",
            @"panorama\images\hud\reborn\icon_speed_psd.vtex_c");

        this.enabled.ValueChange += this.EnabledOnValueChange;
    }

    public void Dispose()
    {
        this.enabled.ValueChange -= this.EnabledOnValueChange;
        this.enabledDamage.ValueChange -= this.EnabledDamageOnValueChange;
        this.toggleKey.ValueChange -= this.ToggleKeyOnValueChange;
        this.holdKey.ValueChange -= this.HoldKeyOnValueChange;
        RendererManager.Draw -= this.OnDraw;
        UpdateManager.DestroyIngameUpdate(this.OnUpdate);
        EntityManager9.UnitAdded -= this.OnUnitAdded;
        EntityManager9.UnitRemoved -= this.OnUnitRemoved;
        ModifierManager.ModifierAdded -= this.OnModifierAdded;
        ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        this.position.Dispose();
        this.units.Clear();
    }

    private void EnabledDamageOnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            UpdateManager.CreateIngameUpdate(1000, this.OnUpdate);
            ModifierManager.ModifierAdded += this.OnModifierAdded;
            ModifierManager.ModifierRemoved += this.OnModifierRemoved;
        }
        else
        {
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        }
    }

    private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            this.toggleKey.ValueChange += this.ToggleKeyOnValueChange;
            this.holdKey.ValueChange += this.HoldKeyOnValueChange;
            EntityManager9.UnitAdded += this.OnUnitAdded;
            EntityManager9.UnitRemoved += this.OnUnitRemoved;
        }
        else
        {
            this.toggleKey.ValueChange -= this.ToggleKeyOnValueChange;
            this.holdKey.ValueChange -= this.HoldKeyOnValueChange;
            this.enabledDamage.ValueChange -= this.EnabledDamageOnValueChange;
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.UnitRemoved -= this.OnUnitRemoved;
            RendererManager.Draw -= this.OnDraw;
            this.units.Clear();
        }
    }

    private void HoldKeyOnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
    {
        this.toggleKey.IsActive = e.NewValue;
    }

    private void OnDraw()
    {
        if (GameManager.IsShopOpen && this.hudMenu.DontDrawWhenShopIsOpen)
        {
            return;
        }

        try
        {
            var mySpeed = this.owner.Hero.Speed;

            foreach (var unit in this.units)
            {
                if (!unit.ShouldDraw)
                {
                    continue;
                }

                unit.DrawInformation(mySpeed, this.enabledDamage, this.enabledSpeed, this.position, this.size);
            }
        }
        catch (InvalidOperationException)
        {
            // ignore
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnModifierAdded(ModifierAddedEventArgs e)
    {
        OnModifierChange(e.Modifier);
    }

    private void OnModifierRemoved(ModifierRemovedEventArgs e)
    {
        OnModifierChange(e.Modifier);
    }

    private void OnModifierChange(Modifier modifier)
    {
        try
        {
            if (modifier.IsHidden)
            {
                return;
            }

            var sender = modifier.Owner;

            if (sender.Handle == this.owner.HeroHandle)
            {
                UpdateManager.BeginInvoke(this.OnUpdate);

                return;
            }

            var unit = this.units.Find(x => x.Unit.Handle == sender.Handle);

            if (unit != null)
            {
                UpdateManager.BeginInvoke(
                    () =>
                    {
                        try
                        {
                            unit.UpdateDamage(this.owner.Hero);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    });
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUnitAdded(Unit9 unit)
    {
        try
        {
            if (!unit.IsHero || unit.IsIllusion || !unit.IsEnemy(this.owner.Team))
            {
                return;
            }

            this.units.Add(new InformationUnit(unit));
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUnitRemoved(Unit9 unit)
    {
        try
        {
            if (!unit.IsHero || unit.IsIllusion || !unit.IsEnemy(this.owner.Team))
            {
                return;
            }

            var autoAttackUnit = this.units.Find(x => x.Unit.Handle == unit.Handle);

            if (autoAttackUnit != null)
            {
                this.units.Remove(autoAttackUnit);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUpdate()
    {
        try
        {
            var myHero = this.owner.Hero;

            for (var i = this.units.Count - 1; i > -1; i--)
            {
                var unit = this.units[i];

                if (!unit.Unit.IsValid)
                {
                    this.units.RemoveAt(i);

                    continue;
                }

                unit.UpdateDamage(myHero);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void ToggleKeyOnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
    {
        if (e.NewValue)
        {
            RendererManager.Draw += this.OnDraw;
            this.enabledDamage.ValueChange += this.EnabledDamageOnValueChange;
        }
        else
        {
            RendererManager.Draw -= this.OnDraw;
            this.enabledDamage.ValueChange -= this.EnabledDamageOnValueChange;
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.UnitRemoved -= this.OnUnitRemoved;
        }
    }
}