namespace O9K.Hud.Modules.Map.Runes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Data;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Menu;
    using Core.Managers.Menu.Items;

    using Divine;
    using Divine.SDK.Extensions;

    using Helpers;

    using MainMenu;

    internal class PowerRunes : IHudModule
    {
        private readonly IMinimap minimap;

        private readonly Dictionary<RuneType, string> runeTextures = new Dictionary<RuneType, string>
        {
            { RuneType.DoubleDamage, "rune_double_damage_minimap" },
            { RuneType.Invisibility, "rune_invisibility_minimap" },
            { RuneType.Arcane, "rune_arcane_minimap" },
            { RuneType.Haste, "rune_haste_minimap" },
            { RuneType.Illusion, "rune_illusion_minimap" },
            { RuneType.Regeneration, "rune_regeneration_minimap" },
        };

        private readonly MenuSwitcher showOnMap;

        private readonly MenuSwitcher showOnMinimap;

        private RuneData[] runeSpawns;

        public PowerRunes(IMinimap minimap, IHudMenu hudMenu)
        {
            this.minimap = minimap;

            var runesMenu = hudMenu.MapMenu.GetOrAdd(new Menu("Runes"));
            runesMenu.AddTranslation(Lang.Ru, "Руны");
            runesMenu.AddTranslation(Lang.Cn, "神符");

            var menu = runesMenu.Add(new Menu("Power-up"));
            menu.AddTranslation(Lang.Ru, "Обычные");
            menu.AddTranslation(Lang.Cn, "强化神符");

            this.showOnMap = menu.Add(new MenuSwitcher("Show on map", false)).SetTooltip("Show cached/picked power-up runes");
            this.showOnMap.AddTranslation(Lang.Ru, "Показывать на карте");
            this.showOnMap.AddTooltipTranslation(Lang.Ru, "Показывать запомненые и взятые руны");
            this.showOnMap.AddTranslation(Lang.Cn, "地图上显示");
            this.showOnMap.AddTooltipTranslation(Lang.Cn, "显示缓存/挑选的能力符文");

            this.showOnMinimap = menu.Add(new MenuSwitcher("Show on minimap")).SetTooltip("Show cached/picked power-up runes");
            this.showOnMinimap.AddTranslation(Lang.Ru, "Показывать на миникарте");
            this.showOnMinimap.AddTooltipTranslation(Lang.Ru, "Показывать запомненые и взятые руны");
            this.showOnMinimap.AddTranslation(Lang.Cn, "小地图上显示");
            this.showOnMinimap.AddTooltipTranslation(Lang.Cn, "显示缓存/挑选的能力符文");
        }

        public void Activate()
        {
            this.LoadTextures();

            this.runeSpawns = EntityManager.GetEntities<Item>()
                .Where(x => x.NetworkName == "CDOTA_Item_RuneSpawner_Powerup")
                .Select(x => new RuneData(x.Position))
                .ToArray();

            if (this.runeSpawns.Length == 0)
            {
                return;
            }

            EntityManager.EntityAdded += this.OnAddEntity;
            EntityManager.EntityRemoved += this.OnRemoveEntity;
            UpdateManager.CreateIngameUpdate(3000, this.OnUpdate);

            RendererManager.Draw += this.OnDraw;
        }

        public void Dispose()
        {
            EntityManager.EntityAdded -= this.OnAddEntity;
            EntityManager.EntityRemoved -= this.OnRemoveEntity;
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);

            RendererManager.Draw -= this.OnDraw;
        }

        private void LoadTextures()
        {
            RendererManager.LoadTextureFromAssembly("rune_regeneration_minimap", "rune_regeneration_minimap.png");
            RendererManager.LoadTextureFromAssembly("rune_arcane_minimap", "rune_arcane_minimap.png");
            RendererManager.LoadTextureFromAssembly("rune_double_damage_minimap", "rune_double_damage_minimap.png");
            RendererManager.LoadTextureFromAssembly("rune_haste_minimap", "rune_haste_minimap.png");
            RendererManager.LoadTextureFromAssembly("rune_illusion_minimap", "rune_illusion_minimap.png");
            RendererManager.LoadTextureFromAssembly("rune_invisibility_minimap", "rune_invisibility_minimap.png");
        }

        private void OnAddEntity(EntityAddedEventArgs e)
        {
            try
            {
                var rune = e.Entity as Rune;
                if (rune == null || rune.RuneType == RuneType.Bounty)
                {
                    return;
                }

                var spawn = this.runeSpawns.Find(x => x.Position.Distance2D(rune.Position) < 100);
                spawn?.AddRune(rune, this.runeTextures[rune.RuneType]);
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
                foreach (var rune in this.runeSpawns)
                {
                    if (!rune.Display)
                    {
                        continue;
                    }

                    if (this.showOnMinimap)
                    {
                        var position = this.minimap.WorldToMinimap(rune.Position, 20 * Hud.Info.ScreenRatio);
                        RendererManager.DrawTexture(rune.RunePicked ? "o9k.x" : rune.Texture, position);
                    }

                    if (this.showOnMap)
                    {
                        var position = this.minimap.WorldToScreen(rune.Position, 40 * Hud.Info.ScreenRatio);
                        if (position.IsZero)
                        {
                            continue;
                        }

                        RendererManager.DrawTexture(rune.RunePicked ? "o9k.x" : rune.Texture, position);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnRemoveEntity(EntityRemovedEventArgs e)
        {
            try
            {
                var rune = e.Entity as Rune;
                if (rune == null || rune.RuneType == RuneType.Bounty)
                {
                    return;
                }

                var spawn = this.runeSpawns.Find(x => x.Position.Distance2D(rune.Position) < 100);
                spawn?.RemoveRune(rune);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnUpdate()
        {
            try
            {
                if (GameManager.GameTime % GameData.RuneRespawnTime > GameData.RuneRespawnTime - 3)
                {
                    foreach (var rune in this.runeSpawns)
                    {
                        rune.Display = false;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}