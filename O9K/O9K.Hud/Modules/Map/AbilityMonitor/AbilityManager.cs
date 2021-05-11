namespace O9K.Hud.Modules.Map.AbilityMonitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities.Base;
    using Abilities.Data.UniqueAbilities.Wards;

    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine;
    using Divine.SDK.Extensions;

    using Helpers;
    using Helpers.Notificator;

    using MainMenu;

    using SharpDX;

    using AbilityData = Abilities.Data.AbilityData;

    internal class AbilityManager : IHudModule
    {
        private readonly List<IDrawableAbility> drawableAbilities = new List<IDrawableAbility>();

        private readonly MenuSwitcher enabled;

        private readonly List<EnemyUnit> enemyUnits = new List<EnemyUnit>();

        private readonly IMinimap minimap;

        private readonly MenuSwitcher notificationsEnabled;

        private readonly INotificator notificator;

        private readonly MenuSwitcher showOnMap;

        private readonly MenuSwitcher showOnMinimap;

        private AbilityData abilityData;

        private Team allyTeam;

        public AbilityManager(IMinimap minimap, INotificator notificator, IHudMenu hudMenu)
        {
            this.minimap = minimap;
            this.notificator = notificator;

            var abilitiesMenu = hudMenu.MapMenu.Add(new Menu("Abilities"));
            abilitiesMenu.AddTranslation(Lang.Ru, "Способности");
            abilitiesMenu.AddTranslation(Lang.Cn, "技能");

            this.enabled = abilitiesMenu.Add(new MenuSwitcher("Enabled")).SetTooltip("Show used enemy abilities in fog");
            this.enabled.AddTranslation(Lang.Ru, "Включено");
            this.enabled.AddTooltipTranslation(Lang.Ru, "Показывать использованные способности врага в тумане");
            this.enabled.AddTranslation(Lang.Cn, "启用");
            this.enabled.AddTooltipTranslation(Lang.Cn, "在雾中显示使用过的敌人能力");

            this.showOnMinimap = abilitiesMenu.Add(new MenuSwitcher("Show on minimap"));
            this.showOnMinimap.AddTranslation(Lang.Ru, "Показывать на миникарте");
            this.showOnMinimap.AddTranslation(Lang.Cn, "小地图上显示");

            this.showOnMap = abilitiesMenu.Add(new MenuSwitcher("Show on map"));
            this.showOnMap.AddTranslation(Lang.Ru, "Показывать на карте");
            this.showOnMap.AddTranslation(Lang.Cn, "地图上显示");

            var notificationsMenu = hudMenu.NotificationsMenu.GetOrAdd(new Menu("Abilities"));
            notificationsMenu.AddTranslation(Lang.Ru, "Способности");
            notificationsMenu.AddTranslation(Lang.Cn, "技能");

            var usedMenu = notificationsMenu.GetOrAdd(new Menu("Used"));
            usedMenu.AddTranslation(Lang.Ru, "Использованные");
            usedMenu.AddTranslation(Lang.Cn, "使用");

            this.notificationsEnabled = usedMenu.GetOrAdd(new MenuSwitcher("Enabled")).SetTooltip("Notify about used dangerous abilities");
            this.notificationsEnabled.AddTranslation(Lang.Ru, "Включено");
            this.notificationsEnabled.AddTooltipTranslation(Lang.Ru, "Оповещать об использованных опасных способностях");
            this.notificationsEnabled.AddTranslation(Lang.Cn, "启用");
            this.notificationsEnabled.AddTooltipTranslation(Lang.Cn, "通知使用过的危险能力");
        }

        public void Activate()
        {
            this.LoadTextures();

            this.abilityData = new AbilityData();
            this.allyTeam = EntityManager9.Owner.Team;

            this.enabled.ValueChange += this.EnabledOnValueChange;
        }

        public void Dispose()
        {
            RendererManager.Draw -= this.OnDraw;
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.UnitRemoved -= this.OnUnitRemoved;
            UpdateManager.DestroyIngameUpdate(this.OnUpdateRemove);
            UpdateManager.DestroyIngameUpdate(this.OnUpdateWard);
            ParticleManager.ParticleAdded -= this.OnParticleAdded;
            EntityManager.EntityAdded -= this.OnAddEntity;
            EntityManager.EntityRemoved -= this.OnRemoveEntity;
            this.enabled.ValueChange -= this.EnabledOnValueChange;
        }

        private void AddWard(EnemyUnit enemy, string unitName)
        {
            if (!this.abilityData.Units.TryGetValue(unitName, out var data))
            {
                return;
            }

            var wardPosition = enemy.Unit.InFront(400);

            if (this.drawableAbilities.OfType<DrawableWardAbility>()
                .Any(x => x.Unit != null && x.AbilityUnitName == unitName && x.Position.Distance2D(wardPosition) < 400))
            {
                return;
            }

            if (this.drawableAbilities.OfType<DrawableWardAbility>().Any(x => x.Position.Distance2D(wardPosition) <= 50))
            {
                wardPosition += new Vector3(60, 0, 0);
            }

            ((WardAbilityData)data).AddDrawableAbility(this.drawableAbilities, wardPosition);
        }

        private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                if (e.OldValue)
                {
                    //todo delete
                    if (AppDomain.CurrentDomain.GetAssemblies()
                        .Any(x => !x.GlobalAssemblyCache && x.GetName().Name.Contains("VisionControl")))
                    {
                        Hud.DisplayWarning("O9K.Hud // VisionControl is already included in O9K.Hud");
                    }

                    if (AppDomain.CurrentDomain.GetAssemblies().Any(x => !x.GlobalAssemblyCache && x.GetName().Name.Contains("BeAware")))
                    {
                        Hud.DisplayWarning("O9K.Hud // BeAware is already included in O9K.Hud");
                    }
                }

                EntityManager9.UnitAdded += this.OnUnitAdded;
                EntityManager9.UnitRemoved += this.OnUnitRemoved;
                UpdateManager.CreateIngameUpdate(500, this.OnUpdateRemove);
                UpdateManager.CreateIngameUpdate(200, this.OnUpdateWard);
                ParticleManager.ParticleAdded += this.OnParticleAdded;
                EntityManager.EntityAdded += this.OnAddEntity;
                EntityManager.EntityRemoved += this.OnRemoveEntity;
                RendererManager.Draw += this.OnDraw;

                foreach (var particle in ParticleManager.Particles.Where(x => x.IsValid && x.Name == "particles/units/heroes/hero_wisp/wisp_ambient_entity_tentacles.vpcf"))
                {
                    try
                    {
                        if (!this.abilityData.Particles.TryGetValue(particle.Name, out var data))
                        {
                            return;
                        }

                        try
                        {
                            data.AddDrawableAbility(this.drawableAbilities, particle, this.allyTeam, this.notificationsEnabled ? this.notificator : null);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            else
            {
                RendererManager.Draw -= this.OnDraw;
                EntityManager9.UnitAdded -= this.OnUnitAdded;
                EntityManager9.UnitRemoved -= this.OnUnitRemoved;
                ParticleManager.ParticleAdded -= this.OnParticleAdded;
                EntityManager.EntityAdded -= this.OnAddEntity;
                EntityManager.EntityRemoved -= this.OnRemoveEntity;
                UpdateManager.DestroyIngameUpdate(this.OnUpdateRemove);
                UpdateManager.DestroyIngameUpdate(this.OnUpdateWard);
            }
        }

        private bool GaveWard(EnemyUnit enemy)
        {
            return this.enemyUnits.Any(
                x => x.Unit.IsValid && !x.Equals(enemy) && x.Unit.IsVisible && x.Unit.IsAlive && x.Unit.Distance(enemy.Unit) <= 600
                && x.ObserversCount + x.SentryCount
                < x.CountWards(AbilityId.item_ward_observer) + x.CountWards(AbilityId.item_ward_sentry));
        }

        private void LoadTextures()
        {
            RendererManager.LoadTexture(
                "o9k.minimap_item_ward_observer",
                @"panorama\images\hero_selection\minimap_ward_obs_png.vtex_c",
                new TextureProperties
                {
                    ColorRatio = new Vector4(1f, 0.6f, 0f, 1f)
                });
            RendererManager.LoadTexture(
                "o9k.minimap_item_ward_sentry",
                @"panorama\images\hero_selection\minimap_ward_obs_png.vtex_c",
                new TextureProperties
                {
                    ColorRatio = new Vector4(0.1f, 0.4f, 1f, 1f)
                });
        }

        private void OnAddEntity(EntityAddedEventArgs e)
        {
            try
            {
                var unit = e.Entity as Unit;
                if (unit == null || unit.Team == this.allyTeam)
                {
                    return;
                }

                if (this.abilityData.Units.TryGetValue(unit.Name, out var data))
                {
                    data.AddDrawableAbility(this.drawableAbilities, unit, this.notificationsEnabled ? this.notificator : null);
                }
                else
                {
                    if (unit.NetworkName != "CDOTA_BaseNPC")
                    {
                        return;
                    }

                    var vision = unit.DayVision;
                    if (vision <= 0)
                    {
                        return;
                    }

                    var ids = this.abilityData.AbilityUnitVision.Where(x => x.Value.Vision == unit.DayVision)
                        .ToDictionary(x => x.Key, x => x.Value);
                    var abilities = EntityManager9.Abilities.Where(
                            x => x.Owner.Team != this.allyTeam && x.Owner.CanUseAbilities && ids.ContainsKey(x.Id)
                                 && (!x.Owner.IsVisible || x.TimeSinceCasted < 0.5f + x.ActivationDelay))
                        .ToList();

                    if (abilities.Count != 1)
                    {
                        return;
                    }

                    if (!ids.TryGetValue(abilities[0].Id, out data))
                    {
                        return;
                    }

                    data.AddDrawableAbility(
                        this.drawableAbilities,
                        abilities[0],
                        unit,
                        this.notificationsEnabled ? this.notificator : null);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnDraw()
        {
            try
            {
                foreach (var ability in this.drawableAbilities)
                {
                    try
                    {
                        if (!ability.Draw)
                        {
                            continue;
                        }

                        if (this.showOnMap)
                        {
                            ability.DrawOnMap(this.minimap);
                        }

                        if (this.showOnMinimap)
                        {
                            ability.DrawOnMinimap(this.minimap);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }

                var mines = this.drawableAbilities.Where(x => x.AbilityId == AbilityId.techies_remote_mines).ToArray();
                if (mines.Length < 2)
                {
                    return;
                }

                var ignoredMines = new List<IDrawableAbility>();

                foreach (var mine in mines)
                {
                    if (ignoredMines.Contains(mine))
                    {
                        continue;
                    }

                    var stack = mines.Where(x => x.Position.DistanceSquared(mine.Position) < 10000).ToList();
                    if (stack.Count < 2)
                    {
                        continue;
                    }

                    ignoredMines.AddRange(stack);

                    var position = this.minimap.WorldToScreen(mine.Position, 24 * Hud.Info.ScreenRatio);
                    if (position.IsZero)
                    {
                        continue;
                    }

                    RendererManager.DrawText(
                        "x" + stack.Count,
                        position + new Vector2(40, 10),
                        Color.White,
                        FontFlags.Left,
                        24 * Hud.Info.ScreenRatio);
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

        private void OnParticleAdded(ParticleAddedEventArgs e)
        {
            try
            {
                if (!this.abilityData.Particles.TryGetValue(e.Particle.Name, out var data))
                {
                    return;
                }

                /*if (data.ParticleReleaseData)
                {
                    return;
                }*/

                UpdateManager.BeginInvoke(
                    () =>
                        {
                            try
                            {
                                var particle = e.Particle;
                                if (!particle.IsValid)
                                {
                                    return;
                                }

                                data.AddDrawableAbility(
                                    this.drawableAbilities,
                                    particle,
                                    this.allyTeam,
                                    this.notificationsEnabled ? this.notificator : null);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                            }
                        });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /*private void OnParticleReleased(Entity sender, ParticleReleasedEventArgs args)
        {
            try
            {
                if (!args.Particle.IsValid)
                {
                    return;
                }

                if (!this.abilityData.Particles.TryGetValue(args.Particle.Name, out var data))
                {
                    return;
                }

                if (!data.ParticleReleaseData)
                {
                    return;
                }

                data.AddDrawableAbility(
                    this.drawableAbilities,
                    args.Particle,
                    this.allyTeam,
                    this.notificationsEnabled ? this.notificator : null);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }*/

        private void OnRemoveEntity(EntityRemovedEventArgs e)
        {
            try
            {
                var entity = e.Entity;
                if (entity.Team == this.allyTeam)
                {
                    return;
                }

                if (!this.abilityData.Units.ContainsKey(entity.Name))
                {
                    return;
                }

                var unit = this.drawableAbilities.OfType<DrawableUnitAbility>().FirstOrDefault(x => x.Unit == entity);
                if (unit != null)
                {
                    this.drawableAbilities.Remove(unit);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnUnitAdded(Unit9 entity)
        {
            try
            {
                if ((!entity.IsHero && !entity.IsCourier) || !entity.CanUseAbilities || entity.Team == this.allyTeam)
                {
                    return;
                }

                this.enemyUnits.Add(new EnemyUnit(entity));
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
                if ((!entity.IsHero && !entity.IsCourier) || !entity.CanUseAbilities || entity.Team == this.allyTeam)
                {
                    return;
                }

                var unit = this.enemyUnits.Find(x => x.Unit == entity);
                if (unit != null)
                {
                    this.enemyUnits.Remove(unit);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUpdateRemove()
        {
            for (var i = this.drawableAbilities.Count - 1; i > -1; i--)
            {
                try
                {
                    var ability = this.drawableAbilities[i];

                    if (ability.IsValid)
                    {
                        continue;
                    }

                    if (ability.IsShowingRange)
                    {
                        ability.RemoveRange();
                    }

                    this.drawableAbilities.RemoveAt(i);
                }
                catch
                {
                    this.drawableAbilities.RemoveAt(i);
                }
            }
        }

        private void OnUpdateWard()
        {
            try
            {
                foreach (var enemy in this.enemyUnits)
                {
                    if (!enemy.Unit.IsValid)
                    {
                        continue;
                    }

                    if (!enemy.Unit.IsVisible)
                    {
                        enemy.ObserversCount = 0;
                        enemy.SentryCount = 0;
                        continue;
                    }

                    if (this.PlacedWard(enemy, AbilityId.item_ward_observer))
                    {
                        this.AddWard(enemy, "npc_dota_observer_wards");
                    }
                    else if (this.PlacedWard(enemy, AbilityId.item_ward_sentry))
                    {
                        this.AddWard(enemy, "npc_dota_sentry_wards");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private bool PlacedWard(EnemyUnit enemy, AbilityId id)
        {
            var count = enemy.CountWards(id);
            if (count < enemy.GetWardsCount(id))
            {
                enemy.SetWardsCount(id, count);

                if (!this.GaveWard(enemy) && !enemy.DroppedWard(id) && !enemy.Unit.IsCourier)
                {
                    return true;
                }
            }
            else if (count > enemy.GetWardsCount(id) && !this.TookWard(enemy))
            {
                enemy.SetWardsCount(id, count);
            }

            return false;
        }

        private bool TookWard(EnemyUnit enemy)
        {
            return this.enemyUnits.Any(
                x => x.Unit.IsValid && !x.Equals(enemy) && x.Unit.IsAlive && x.Unit.Distance(enemy.Unit) <= 600
                     && x.ObserversCount + x.SentryCount
                     > x.CountWards(AbilityId.item_ward_observer) + x.CountWards(AbilityId.item_ward_sentry));
        }
    }
}