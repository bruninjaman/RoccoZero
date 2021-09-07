﻿namespace O9K.Hud.Modules.Map.Teleports;

using System;
using System.Collections.Generic;
using System.Linq;

using Core.Entities.Heroes;
using Core.Entities.Units;
using Core.Helpers;
using Core.Logger;
using Core.Managers.Entity;
using Core.Managers.Menu;
using Core.Managers.Menu.Items;

using Divine.Entity;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.EventArgs;
using Divine.Particle.Particles;
using Divine.Renderer;
using Divine.Update;

using Helpers;

using MainMenu;

internal class TeleportMonitor : IHudModule
{
    private const float TeleportCheckDuration = 25;

    private const float TeleportCheckRadius = 1150;

    private readonly Dictionary<Color, int> colors = new Dictionary<Color, int>
    {
        { new Color((int)(255 * 0.2f), (int)(255 * 0.46f), (int)(255 * 1f)), 0 },
        { new Color((int)(255 * 0.4f), (int)(255 * 1f), (int)(255 * 0.75f)), 1 },
        { new Color((int)(255 * 0.75f), (int)(255 * 0f), (int)(255 * 0.75f)), 2 },
        { new Color((int)(255 * 0.95f), (int)(255 * 0.94f), (int)(255 * 0.04f)), 3 },
        { new Color((int)(255 * 1f), (int)(255 * 0.42f), (int)(255 * 0f)), 4 },
        { new Color((int)(255 * 1f), (int)(255 * 0.53f), (int)(255 * 0.76f)), 5 },
        { new Color((int)(255 * 0.63f), (int)(255 * 0.71f), (int)(255 * 0.28f)), 6 },
        { new Color((int)(255 * 0.4f), (int)(255 * 0.85f), (int)(255 * 0.97f)), 7 },
        { new Color((int)(255 * 0f), (int)(255 * 0.51f), (int)(255 * 0.13f)), 8 },
        { new Color((int)(255 * 0.64f), (int)(255 * 0.41f), (int)(255 * 0f)), 9 }
    };

    private readonly IMinimap minimap;

    private readonly MenuSwitcher showOnMap;

    private readonly MenuSwitcher showOnMinimap;

    private readonly List<Teleport> teleports = new List<Teleport>();

    private readonly MultiSleeper<Vector3> teleportSleeper = new MultiSleeper<Vector3>();

    private Team ownerTeam;

    private readonly IHudMenu hudMenu;

    public TeleportMonitor(IMinimap minimap, IHudMenu hudMenu)
    {
        this.minimap = minimap;
        this.hudMenu = hudMenu;

        var menu = hudMenu.MapMenu.Add(new Menu("Teleports"));
        menu.AddTranslation(Lang.Ru, "Телепорты");
        menu.AddTranslation(Lang.Cn, "回城");

        this.showOnMap = menu.Add(new MenuSwitcher("Show on map")).SetTooltip("Show enemy teleport locations");
        this.showOnMap.AddTranslation(Lang.Ru, "Показывать на карте");
        this.showOnMap.AddTooltipTranslation(Lang.Ru, "Показывать телепорты врагов");
        this.showOnMap.AddTranslation(Lang.Cn, "地图上显示");
        this.showOnMap.AddTooltipTranslation(Lang.Cn, "显示敌人的传送地点");

        this.showOnMinimap = menu.Add(new MenuSwitcher("Show on minimap")).SetTooltip("Show enemy teleport locations");
        this.showOnMinimap.AddTranslation(Lang.Ru, "Показывать на миникарте");
        this.showOnMinimap.AddTooltipTranslation(Lang.Ru, "Показывать телепорты врагов");
        this.showOnMinimap.AddTranslation(Lang.Cn, "小地图上显示");
        this.showOnMinimap.AddTooltipTranslation(Lang.Cn, "显示敌人的传送地点");
    }

    public void Activate()
    {
        this.ownerTeam = EntityManager9.Owner.Team;
        ParticleManager.ParticleAdded += this.OnParticleAdded;
    }

    public void Dispose()
    {
        ParticleManager.ParticleAdded -= this.OnParticleAdded;
    }

    private void AddTeleport(Teleport tp)
    {
        if (tp.HeroId == HeroId.npc_dota_hero_base)
        {
            return;
        }

        if (this.teleports.Count == 0)
        {
            RendererManager.Draw += this.OnDraw;
        }

        this.teleports.Add(tp);
    }

    private void CheckTeleport(Particle particle, bool start)
    {
        try
        {
            if (!particle.IsValid)
            {
                return;
            }

            var colorCp = particle.GetControlPoint(2);

            var color = new Color(
                (int)(255 * Math.Round(colorCp.X, 2)),
                (int)(255 * Math.Round(colorCp.Y, 2)),
                (int)(255 * Math.Round(colorCp.Z, 2)));

            if (!this.colors.TryGetValue(color, out var id))
            {
                return;
            }

            var player = EntityManager.GetPlayerById(id);

            if (player == null || player.Hero == null || player.Team == this.ownerTeam)
            {
                return;
            }

            var hero = (Hero9)EntityManager9.GetUnit(player.Hero.Handle);

            if (hero == null || (hero.IsVisible && start))
            {
                return;
            }

            var position = particle.GetControlPoint(0);
            var heroId = hero.Id;
            var duration = 3f;

            //  if (hero.Abilities.Any(x => x.Id == AbilityId.item_travel_boots && x.CanBeCasted(false)))
            {
                duration = this.GetDuration(heroId, position, start);
            }

            this.AddTeleport(new Teleport(particle, heroId, position, duration, start));

            if (start)
            {
                hero.ChangeBasePosition(position);
            }
            else
            {
                UpdateManager.BeginInvoke((int)(duration * 1000) - 200, () => this.SetPosition(particle, position, hero));
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private float GetDuration(HeroId heroId, Vector3 position, bool start)
    {
        if (start)
        {
            var end = this.teleports.LastOrDefault(x => x.HeroId == heroId);

            if (end == null)
            {
                return 3f;
            }

            return end.RemainingDuration;
        }

        var duration = 3f;

        if (EntityManager9.EnemyFountain.Distance2D(position) < 1000)
        {
            return duration;
        }

        if (EntityManager9.RadiantOutpost.Distance2D(position) < 1000 || EntityManager9.DireOutpost.Distance2D(position) < 1000)
        {
            duration = 6f;
        }

        var sleepers = this.teleportSleeper.Count(x => x.Value.IsSleeping && x.Key.Distance2D(position) < TeleportCheckRadius);

        if (sleepers > 0)
        {
            duration += (sleepers * 0.5f) + 1.5f;
        }

        this.teleportSleeper[position] = new Sleeper(TeleportCheckDuration);

        return duration;
    }

    private void OnDraw()
    {
        if (GameManager.IsShopOpen && this.hudMenu.DontDrawWhenShopIsOpen)
        {
            return;
        }

        try
        {
            for (var i = this.teleports.Count - 1; i > -1; i--)
            {
                var teleport = this.teleports[i];

                if (!teleport.IsValid)
                {
                    this.teleports.RemoveAt(i);

                    if (this.teleports.Count == 0)
                    {
                        RendererManager.Draw -= this.OnDraw;
                    }

                    continue;
                }

                if (this.showOnMinimap)
                {
                    teleport.DrawOnMinimap(this.minimap);
                }

                if (this.showOnMap)
                {
                    teleport.DrawOnMap(this.minimap);
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

    private void OnParticleAdded(ParticleAddedEventArgs e)
    {
        var particle = e.Particle;

        switch (particle.Name)
        {
            case "particles/items2_fx/teleport_start.vpcf":
            case "particles/econ/items/tinker/boots_of_travel/teleport_start_bots.vpcf":
            case "particles/econ/events/ti10/teleport/teleport_start_ti10.vpcf":
            case "particles/econ/events/ti10/teleport/teleport_start_ti10_lvl2.vpcf":
            case "particles/econ/events/ti10/teleport/teleport_start_ti10_lvl3.vpcf":
                UpdateManager.BeginInvoke(100, () => this.CheckTeleport(particle, true));

                break;

            case "particles/items2_fx/teleport_end.vpcf":
            case "particles/econ/items/tinker/boots_of_travel/teleport_end_bots.vpcf":
            case "particles/econ/events/ti10/teleport/teleport_end_ti10.vpcf":
            case "particles/econ/events/ti10/teleport/teleport_end_ti10_lvl2.vpcf":
            case "particles/econ/events/ti10/teleport/teleport_end_ti10_lvl3.vpcf":
                UpdateManager.BeginInvoke(() => this.CheckTeleport(particle, false));

                break;
            //default:
            //{
            //    var name = args.Particle.Name;
            //    if ((name.Contains("teleport_start") || name.Contains("teleport_end")) && !name.Contains("furion"))
            //    {
            //        Logger.Error("TP Particle", name);
            //    }
            //    break;
            //}
        }
    }

    private void SetPosition(Particle particle, Vector3 position, Unit9 unit)
    {
        if (!particle.IsValid)
        {
            return;
        }

        // delay position update to make sure hero is teleported
        UpdateManager.BeginInvoke(500, () => unit.ChangeBasePosition(position));
    }
}