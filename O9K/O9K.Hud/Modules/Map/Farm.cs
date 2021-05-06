namespace O9K.Hud.Modules.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Heroes;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Context;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.Items;
    using Core.Managers.Particle;

    using Divine;
    using Divine.SDK.Extensions;

    using Helpers;

    using MainMenu;

    using SharpDX;

    internal class Farm : IHudModule
    {
        //private readonly HashSet<string> attackParticles = new HashSet<string>
        //{
        //    "particles/generic_gameplay/generic_hit_blood.vpcf",
        //    "particles/neutral_fx/neutral_item_drop_lvl0.vpcf",
        //    "particles/neutral_fx/neutral_item_drop_lvl1.vpcf",
        //    "particles/neutral_fx/neutral_item_drop_lvl2.vpcf",
        //    "particles/neutral_fx/neutral_item_drop_lvl3.vpcf",
        //    "particles/neutral_fx/neutral_item_drop_lvl4.vpcf",
        //    "particles/neutral_fx/neutral_item_drop_lvl5.vpcf"
        //};

        private readonly Dictionary<uint, (HeroId, Vector3, Sleeper)> attacks = new Dictionary<uint, (HeroId, Vector3,Sleeper)>();

        private readonly IMinimap minimap;

        private readonly MenuSwitcher showOnMap;

        private readonly MenuSwitcher showOnMinimap;

        private Owner owner;

        public Farm(IMinimap minimap, IHudMenu hudMenu)
        {
            this.minimap = minimap;

            var menu = hudMenu.MapMenu.Add(new Menu("Farm"));
            menu.AddTranslation(Lang.Ru, "Фарм");
            menu.AddTranslation(Lang.Cn, "打钱");

            this.showOnMap = menu.Add(new MenuSwitcher("Show on map")).SetTooltip("Show when enemy attacks neutrals/roshan");
            this.showOnMap.AddTranslation(Lang.Ru, "Показывать на карте");
            this.showOnMap.AddTooltipTranslation(Lang.Ru, "Показывать когда враг атакует нейтралов/рошана");
            this.showOnMap.AddTranslation(Lang.Cn, "地图上显示");
            this.showOnMap.AddTooltipTranslation(Lang.Cn, "显示敌人何时袭击野区/肉山");

            this.showOnMinimap = menu.Add(new MenuSwitcher("Show on minimap")).SetTooltip("Show when enemy attacks neutrals/roshan");
            this.showOnMinimap.AddTranslation(Lang.Ru, "Показывать на миникарте");
            this.showOnMinimap.AddTooltipTranslation(Lang.Ru, "Показывать когда враг атакует нейтралов/рошана");
            this.showOnMinimap.AddTranslation(Lang.Cn, "小地图上显示");
            this.showOnMinimap.AddTooltipTranslation(Lang.Cn, "显示敌人何时袭击野区/肉山");
        }

        public void Activate()
        {
            this.owner = EntityManager9.Owner;
            RendererManager.LoadTexture("o9k.attack", @"panorama\images\hud\reborn\ping_icon_attack_psd.vtex_c");

            //Context9.ParticleManger.ParticleAdded += this.OnParticleAdded;
            GameManager.GameEvent += OnGameEvent;
        }

        public void Dispose()
        {
            RendererManager.Draw -= this.OnDraw;
            //Context9.ParticleManger.ParticleAdded -= this.OnParticleAdded;
            GameManager.GameEvent -= OnGameEvent;
        }

        private void OnDraw()
        {
            try
            {
                foreach (var attack in this.attacks.ToList())
                {
                    var (heroId, position, sleeper) = attack.Value;

                    if (!sleeper.IsSleeping)
                    {
                        this.attacks.Remove(attack.Key);

                        if (this.attacks.Count == 0)
                        {
                            RendererManager.Draw -= this.OnDraw;
                        }

                        continue;
                    }

                    if (this.showOnMinimap)
                    {
                        var minimapPosition = minimap.WorldToMinimap(position, 25 * Hud.Info.ScreenRatio);
                        if (minimapPosition.IsZero)
                        {
                            return;
                        }

                        RendererManager.DrawTexture("o9k.outline_red", minimapPosition * 1.08f);
                        RendererManager.DrawTexture(heroId, minimapPosition, UnitTextureType.MiniUnit);
                    }

                    if (this.showOnMap)
                    {
                        var screenPosition = this.minimap.WorldToScreen(position, 40 * Hud.Info.ScreenRatio);
                        if (screenPosition.IsZero)
                        {
                            continue;
                        }

                        RendererManager.DrawTexture("o9k.outline_red", screenPosition * 1.12f);
                        RendererManager.DrawTexture(heroId, screenPosition, UnitTextureType.RoundUnit);

                        var attackPosition = screenPosition * 0.5f;
                        attackPosition.X += attackPosition.Width * 0.8f;
                        attackPosition.Y += attackPosition.Height * 0.6f;

                        RendererManager.DrawTexture("o9k.outline_green_pct100", attackPosition * 1.2f);
                        RendererManager.DrawTexture("o9k.attack", attackPosition);
                    }
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

        private void OnGameEvent(GameEventEventArgs e)
        {
            var gameEvent = e.GameEvent;
            if (gameEvent.Name != "entity_hurt")
            {
                return;
            }

            var attacked = EntityManager.GetEntityByIndex(gameEvent.GetInt32("entindex_killed"));
            if (attacked == null || attacked is not Neutral and not Roshan || attacked.Team != Team.Neutral)
            {
                return;
            }

            var attacker = EntityManager.GetEntityByIndex(gameEvent.GetInt32("entindex_attacker"));
            if (attacker == null || attacker is not Hero attackerHero || attacker.IsVisible)
            {
                return;
            }

            //if (attacks.TryGetValue(attackerHandle, out var value))
            //{
            //    value.Item3.Sleep(3);
            //    return;
            //}

            if (this.attacks.Count == 0)
            {
                RendererManager.Draw += this.OnDraw;
            }

            this.attacks[attacker.Handle] = (attackerHero.HeroId, attacked.Position, new Sleeper(3));
        }

        //private void OnParticleAdded(Particle9 particle)
        //{
        //    try
        //    {
        //        if (/*!particle.Released || */particle.SenderHandle == null)
        //        {
        //            return;
        //        }

        //        if (!this.attackParticles.Contains(particle.Name))
        //        {
        //            return;
        //        }

        //        var position = particle.GetControlPoint(0);

        //        if (particle.SenderHandle == this.owner.PlayerHandle)
        //        {
        //            var droppedItem = EntityManager.GetEntities<PhysicalItem>().FirstOrDefault(x => x.Distance2D(position) < 300);
        //            if (droppedItem != null)
        //            {
        //                return;
        //            }
        //        }
        //        else if (particle.Sender.Team != Team.Neutral || particle.Sender.IsVisible)
        //        {
        //            return;
        //        }

        //        var sleeper = this.attacks.FirstOrDefault(x => x.Key.Distance2D(position) < 500).Value;
        //        if (sleeper != null)
        //        {
        //            sleeper.Sleep(3);
        //            return;
        //        }

        //        if (this.attacks.Count == 0)
        //        {
        //            RendererManager.Draw += this.OnDraw;
        //        }

        //        this.attacks[position] = new Sleeper(3);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error(e);
        //    }
        //}
    }
}