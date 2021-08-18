﻿namespace O9K.Hud.Modules.Map.Predictions.Heroes
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine.Entity.Entities.Components;
    using Divine.Game;
    using Divine.Renderer;

    using Helpers;

    using MainMenu;

    internal class Heroes : IHudModule
    {
        private readonly MenuSwitcher enabled;

        private readonly IMinimap minimap;

        private readonly MenuSwitcher showOnMap;

        private readonly MenuSwitcher showOnMinimap;

        private readonly List<Unit9> units = new List<Unit9>();

        private Team ownerTeam;

        private readonly IHudMenu hudMenu;

        public Heroes(IMinimap minimap, IHudMenu hudMenu)
        {
            this.minimap = minimap;
            this.hudMenu = hudMenu;

            var predictionsMenu = hudMenu.MapMenu.GetOrAdd(new Menu("Predictions"));
            predictionsMenu.AddTranslation(Lang.Ru, "Предположения");
            predictionsMenu.AddTranslation(Lang.Cn, "预测");

            var menu = predictionsMenu.Add(new Menu("Heroes"));
            menu.AddTranslation(Lang.Ru, "Герои");
            menu.AddTranslation(Lang.Cn, "英雄");

            this.enabled = menu.Add(new MenuSwitcher("Enabled", false));
            this.enabled.AddTranslation(Lang.Ru, "Включено");
            this.enabled.AddTranslation(Lang.Cn, "启用");

            this.showOnMap = menu.Add(new MenuSwitcher("Show on map")).SetTooltip("Show predicted hero position on map");
            this.showOnMap.AddTranslation(Lang.Ru, "Показывать на карте");
            this.showOnMap.AddTooltipTranslation(Lang.Ru, "Показывать предполагаемую позицию на карте");
            this.showOnMap.AddTranslation(Lang.Cn, "地图上显示");
            this.showOnMap.AddTooltipTranslation(Lang.Cn, "在地图上显示预测的英雄位置");

            this.showOnMinimap = menu.Add(new MenuSwitcher("Show on minimap")).SetTooltip("Show predicted hero position on minimap");
            this.showOnMinimap.AddTranslation(Lang.Ru, "Показывать на миникарте");
            this.showOnMinimap.AddTooltipTranslation(Lang.Ru, "Показывать предполагаемую позицию на миникарте");
            this.showOnMinimap.AddTranslation(Lang.Cn, "小地图上显示");
            this.showOnMinimap.AddTooltipTranslation(Lang.Cn, "在小地图上显示预测的英雄位置");
        }

        public void Activate()
        {
            this.ownerTeam = EntityManager9.Owner.Team;

            this.enabled.ValueChange += this.EnabledOnValueChange;
        }

        public void Dispose()
        {
            this.enabled.ValueChange -= this.EnabledOnValueChange;
            RendererManager.Draw -= this.OnDraw;
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.UnitRemoved -= this.OnUnitRemoved;
        }

        private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                EntityManager9.UnitAdded += this.OnUnitAdded;
                EntityManager9.UnitRemoved += this.OnUnitRemoved;
                RendererManager.Draw += this.OnDraw;
            }
            else
            {
                EntityManager9.UnitAdded -= this.OnUnitAdded;
                EntityManager9.UnitRemoved -= this.OnUnitRemoved;
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
                foreach (var unit in this.units)
                {
                    if (!unit.IsValid || unit.IsVisible || !unit.IsAlive)
                    {
                        continue;
                    }

                    var position = unit.GetPredictedPosition(0, true);

                    if (this.showOnMinimap)
                    {
                        var size = 25 * Hud.Info.ScreenRatio;
                        var minimapPosition = this.minimap.WorldToMinimap(position, size);

                        RendererManager.DrawImage("o9k.outline_yellow", minimapPosition * 1.08f);
                        RendererManager.DrawImage(unit.Name, minimapPosition, UnitImageType.MiniUnit);
                    }

                    if (this.showOnMap)
                    {
                        var size = 45 * Hud.Info.ScreenRatio;
                        var mapPosition = this.minimap.WorldToScreen(position, size);

                        if (mapPosition.IsZero)
                        {
                            continue;
                        }

                        RendererManager.DrawImage("o9k.outline_yellow", mapPosition * 1.15f);
                        RendererManager.DrawImage(unit.Name, mapPosition, UnitImageType.RoundUnit);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // ignored
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUnitAdded(Unit9 entity)
        {
            try
            {
                if (!entity.IsHero || entity.IsIllusion || entity.Team == this.ownerTeam)
                {
                    return;
                }

                this.units.Add(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUnitRemoved(Unit9 entity)
        {
            try
            {
                if (!entity.IsHero || entity.IsIllusion || entity.Team == this.ownerTeam)
                {
                    return;
                }

                this.units.Remove(entity);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}