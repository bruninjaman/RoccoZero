﻿namespace O9K.Hud.Modules.Screen.Panels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Heroes.Unique;
    using Core.Entities.Units;
    using Core.Entities.Units.Unique;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine.Entity.Entities.Components;
    using Divine.Game;
    using Divine.Numerics;
    using Divine.Renderer;

    using Helpers;

    using MainMenu;

    internal class NetWorthPanel : IHudModule
    {
        private readonly MenuSwitcher allies;

        private readonly MenuSwitcher enemies;

        private readonly MenuHoldKey holdKey;

        private readonly MenuVectorSlider position;

        private readonly MenuSwitcher show;

        private readonly MenuSlider size;

        private readonly MenuToggleKey toggleKey;

        private readonly Dictionary<Unit9, int> units = new Dictionary<Unit9, int>();

        private Vector2 heroSize;

        private Vector2 lineSize;

        private Team ownerTeam;

        private float textSize;

        private readonly IHudMenu hudMenu;

        public NetWorthPanel(IHudMenu hudMenu)
        {
            this.hudMenu = hudMenu;

            var panelsMenu = hudMenu.ScreenMenu.GetOrAdd(new Menu("Panels"));
            panelsMenu.AddTranslation(Lang.Ru, "Панели");
            panelsMenu.AddTranslation(Lang.Cn, "面板");

            var menu = panelsMenu.Add(new Menu("Net worth panel"));
            menu.AddTranslation(Lang.Ru, "Панель стоимости");
            menu.AddTranslation(Lang.Cn, "净值面板");

            this.show = menu.Add(new MenuSwitcher("Enabled", "enabled", false)).SetTooltip("Show net worth of the heroes");
            this.show.AddTranslation(Lang.Ru, "Включено");
            this.show.AddTooltipTranslation(Lang.Ru, "Показывать панель стоимости героев");
            this.show.AddTranslation(Lang.Cn, "启用");
            this.show.AddTooltipTranslation(Lang.Cn, "显示英雄的净资产");

            this.allies = menu.Add(new MenuSwitcher("Allies", "allies"));
            this.allies.AddTranslation(Lang.Ru, "Союзники");
            this.allies.AddTranslation(Lang.Cn, "盟友");

            this.enemies = menu.Add(new MenuSwitcher("Enemies", "enemies"));
            this.enemies.AddTranslation(Lang.Ru, "Враги");
            this.enemies.AddTranslation(Lang.Cn, "敌人");

            var settings = menu.Add(new Menu("Settings"));
            settings.AddTranslation(Lang.Ru, "Настройки");
            settings.AddTranslation(Lang.Cn, "设置");

            this.size = settings.Add(new MenuSlider("Size", "size", 25, 20, 60));
            this.size.AddTranslation(Lang.Ru, "Размер");
            this.size.AddTranslation(Lang.Cn, "大小");

            this.position = new MenuVectorSlider(settings, new Vector2(Hud.Info.ScreenSize.X * 0.19f, Hud.Info.ScreenSize.Y * 0.75f));

            var keys = menu.Add(new Menu("Keys"));
            keys.AddTranslation(Lang.Ru, "Клавишы");
            keys.AddTranslation(Lang.Cn, "键");

            this.toggleKey = keys.Add(new MenuToggleKey("Toggle key", "toggle")).SetTooltip("Show/hide net worth panel");
            this.toggleKey.AddTranslation(Lang.Ru, "Клавиша переключения");
            this.toggleKey.AddTooltipTranslation(Lang.Ru, "Показать/спрятать панель стоимости героев");
            this.toggleKey.AddTranslation(Lang.Cn, "切换键");
            this.toggleKey.AddTooltipTranslation(Lang.Cn, "显示/隐藏净值面板");

            this.holdKey = keys.Add(new MenuHoldKey("Hold key", "hold")).SetTooltip("Show/hide net worth panel");
            this.holdKey.AddTranslation(Lang.Ru, "Клавиша удержания");
            this.holdKey.AddTooltipTranslation(Lang.Ru, "Показать/спрятать панель стоимости героев");
            this.holdKey.AddTranslation(Lang.Cn, "按住键");
            this.holdKey.AddTooltipTranslation(Lang.Cn, "显示/隐藏净值面板");
        }

        public void Activate()
        {
            RendererManager.LoadImage(
                "o9k.net_worth_bg_ally",
                @"panorama\images\masks\gradient_leftright_png.vtex_c",
                new ImageProperties
                {
                    Brightness = -80,
                    ColorTint = new Color(0, 255, 0, 229)
                });

            RendererManager.LoadImage(
                "o9k.net_worth_bg_enemy",
                @"panorama\images\masks\gradient_leftright_png.vtex_c",
                new ImageProperties
                {
                    Brightness = -80,
                    ColorTint = new Color(255, 0, 0, 229)
                });

            this.ownerTeam = EntityManager9.Owner.Team;

            this.show.ValueChange += this.ShowOnValueChange;
            this.size.ValueChange += this.SizeOnValueChange;
        }

        public void Dispose()
        {
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.UnitRemoved -= this.OnUnitRemoved;
            EntityManager9.AbilityAdded -= this.OnAbilityAdded;
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
            RendererManager.Draw -= this.OnDraw;
            this.show.ValueChange -= this.ShowOnValueChange;
            this.size.ValueChange -= this.SizeOnValueChange;
            this.toggleKey.ValueChange -= this.ToggleKeyOnValueChange;
            this.holdKey.ValueChange -= this.HoldKeyOnValueChange;
            this.position.Dispose();
        }

        private void HoldKeyOnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
        {
            this.toggleKey.IsActive = e.NewValue;
        }

        private void OnAbilityAdded(Ability9 ability)
        {
            try
            {
                if (!ability.IsItem)
                {
                    return;
                }

                var owner = ability.Owner;

                if (owner is SpiritBear)
                {
                    owner = owner.Owner;

                    if (owner == null)
                    {
                        return;
                    }
                }

                if (!this.units.ContainsKey(owner))
                {
                    return;
                }

                this.units[owner] += (int)ability.BaseItem.Cost;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAbilityRemoved(Ability9 ability)
        {
            try
            {
                if (!ability.IsItem)
                {
                    return;
                }

                var owner = ability.Owner;

                if (owner is SpiritBear)
                {
                    owner = owner.Owner;

                    if (owner == null)
                    {
                        return;
                    }
                }

                if (!this.units.ContainsKey(owner))
                {
                    return;
                }

                this.units[owner] -= (int)ability.BaseItem.Cost;
            }
            catch (Exception e)
            {
                Logger.Error(e);
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
                var heroPosition = this.position.Value;

                foreach (var pair in this.units.OrderByDescending(x => x.Value))
                {
                    var unit = pair.Key;

                    if (!unit.IsValid)
                    {
                        continue;
                    }

                    if (unit.Team == this.ownerTeam)
                    {
                        if (!this.allies)
                        {
                            continue;
                        }

                        RendererManager.DrawImage("o9k.net_worth_bg_ally", new RectangleF(heroPosition.X, heroPosition.Y, this.lineSize.X, this.lineSize.Y));
                    }
                    else
                    {
                        if (!this.enemies)
                        {
                            continue;
                        }

                        RendererManager.DrawImage("o9k.net_worth_bg_enemy", new RectangleF(heroPosition.X, heroPosition.Y, this.lineSize.X, this.lineSize.Y));
                    }

                    RendererManager.DrawImage(unit.Name, new RectangleF(heroPosition.X, heroPosition.Y, this.heroSize.X, this.heroSize.Y), UnitImageType.Default);

                    RendererManager.DrawText(
                        pair.Value.ToString("N0"),
                        heroPosition + new Vector2(this.heroSize.X + 5, (this.lineSize.Y - this.textSize) / 5),
                        Color.White,
                        this.textSize);

                    heroPosition += new Vector2(0, this.heroSize.Y + 1);
                }
            }
            catch (InvalidOperationException)
            {
                //ignore
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
                if (!unit.IsHero || unit.IsIllusion)
                {
                    return;
                }

                if (unit is Meepo meepo && !meepo.IsMainMeepo)
                {
                    return;
                }

                this.units[unit] = 0;
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
                if (!unit.IsHero || unit.IsIllusion)
                {
                    return;
                }

                this.units.Remove(unit);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void ShowOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                EntityManager9.UnitAdded += this.OnUnitAdded;
                EntityManager9.UnitRemoved += this.OnUnitRemoved;
                EntityManager9.AbilityAdded += this.OnAbilityAdded;
                EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
                this.toggleKey.ValueChange += this.ToggleKeyOnValueChange;
                this.holdKey.ValueChange += this.HoldKeyOnValueChange;
            }
            else
            {
                EntityManager9.UnitAdded -= this.OnUnitAdded;
                EntityManager9.UnitRemoved -= this.OnUnitRemoved;
                EntityManager9.AbilityAdded -= this.OnAbilityAdded;
                EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
                this.toggleKey.ValueChange -= this.ToggleKeyOnValueChange;
                this.holdKey.ValueChange -= this.HoldKeyOnValueChange;
                RendererManager.Draw -= this.OnDraw;
                this.units.Clear();
            }
        }

        private void SizeOnValueChange(object sender, SliderEventArgs e)
        {
            this.heroSize = new Vector2(e.NewValue * 1.5f, e.NewValue);
            this.lineSize = new Vector2(e.NewValue * 5.5f, e.NewValue);
            this.textSize = e.NewValue * 0.7f;
        }

        private void ToggleKeyOnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
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
    }
}