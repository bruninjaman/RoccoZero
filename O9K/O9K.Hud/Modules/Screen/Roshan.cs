namespace O9K.Hud.Modules.Screen
{
    using System;
    using System.Linq;
    using System.Windows.Input;

    using Core.Data;
    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;
    using Core.Managers.Renderer.Utils;

    using Divine;

    using Helpers;

    using MainMenu;

    using SharpDX;

    using Drawer = Helpers.Drawer;

    internal class RoshanTimer : IHudModule
    {
        private readonly MenuHoldKey altKey;

        private readonly Sleeper clickSleeper = new Sleeper();

        private readonly MenuSwitcher enabled;

        private readonly MenuSwitcher hide;

        private readonly MenuSwitcher printTime;

        private readonly AbilityId[][] roshanDrop =
        {
            new[] { AbilityId.item_aegis },
            new[] { AbilityId.item_aegis, AbilityId.item_cheese, AbilityId.item_aghanims_shard_roshan },
            new[] { AbilityId.item_aegis, AbilityId.item_cheese, AbilityId.item_ultimate_scepter_roshan },
            new[] { AbilityId.item_aegis, AbilityId.item_cheese, AbilityId.item_refresher_shard, AbilityId.item_ultimate_scepter_roshan }
        };

        private readonly MenuHoldKey showDrop;

        private readonly MenuSwitcher showMinTime;

        private readonly MenuSwitcher showRemaining;

        private readonly MenuVectorSlider textPosition;

        private readonly MenuSlider textSize;

        private readonly ITopPanel topPanel;

        private float aegisPickUpTime = float.MinValue;

        private Unit9 roshan;

        private float roshanKillTime = float.MinValue;

        private int roshansKilled;

        private float maximumHealth;

        private float health;

        private float lastTime;

        private readonly Sleeper attackedSleeper = new();
        
        private readonly Sleeper showSleeper = new();

        public RoshanTimer(ITopPanel topPanel, IHudMenu hudMenu)
        {
            this.topPanel = topPanel;

            var menu = hudMenu.ScreenMenu.GetOrAdd(new Menu("Roshan"));
            menu.AddTranslation(Lang.Ru, "Рошан");
            menu.AddTranslation(Lang.Cn, "肉山");

            this.enabled = menu.Add(new MenuSwitcher("Enabled"));
            this.enabled.AddTranslation(Lang.Ru, "Включено");
            this.enabled.AddTranslation(Lang.Cn, "启用");

            this.showRemaining = menu.Add(new MenuSwitcher("Remaining time")).SetTooltip("Show remaining time or respawn time");
            this.showRemaining.AddTranslation(Lang.Ru, "Оставшееся время");
            this.showRemaining.AddTooltipTranslation(Lang.Ru, "Показывать оставшееся время или время спавна");
            this.showRemaining.AddTranslation(Lang.Cn, "剩余时间");
            this.showRemaining.AddTooltipTranslation(Lang.Cn, "显示剩余时间或重生时间");

            this.showMinTime = menu.Add(new MenuSwitcher("Minimum time")).SetTooltip("Show minimum respawn time");
            this.showMinTime.AddTranslation(Lang.Ru, "Минимальное время");
            this.showMinTime.AddTooltipTranslation(Lang.Ru, "Показать минимальное время спавна");
            this.showMinTime.AddTranslation(Lang.Cn, "最小时间");
            this.showMinTime.AddTooltipTranslation(Lang.Cn, "显示最短重生时间");

            this.hide = menu.Add(new MenuSwitcher("Auto hide")).SetTooltip("Hide timer if roshan is spawned");
            this.hide.AddTranslation(Lang.Ru, "Прятать автоматически");
            this.hide.AddTooltipTranslation(Lang.Ru, "Спрятать, если рошан появился");
            this.hide.AddTranslation(Lang.Cn, "自动隐藏");
            this.hide.AddTooltipTranslation(Lang.Cn, "如果生成肉山，则隐藏计时器");

            this.printTime = menu.Add(new MenuSwitcher("Print time on click")).SetTooltip("Print respawn time in chat when clicked");
            this.printTime.AddTranslation(Lang.Ru, "Написать время при нажатии");
            this.printTime.AddTooltipTranslation(Lang.Ru, "Написать время возрождения в чате при нажатии");
            this.printTime.AddTranslation(Lang.Cn, "按下时的写入时间");
            this.printTime.AddTooltipTranslation(Lang.Cn, "单击时打印聊天中的重生时间");

            this.showDrop = menu.Add(new MenuHoldKey("Show drop", Key.LeftAlt)).SetTooltip("Show current/next roshan items");
            this.showDrop.AddTranslation(Lang.Ru, "Показать дроп");
            this.showDrop.AddTooltipTranslation(Lang.Ru, "Показать текущие/следующие предметы Рошана");
            this.showDrop.AddTranslation(Lang.Cn, "显示放置位置");
            this.showDrop.AddTooltipTranslation(Lang.Cn, "顯示當前/下一個肉山項目");

            var settings = menu.Add(new Menu("Settings"));
            settings.AddTranslation(Lang.Ru, "Настройки");
            settings.AddTranslation(Lang.Cn, "设置");

            this.textSize = settings.Add(new MenuSlider("Size", 15, 10, 35));
            this.textSize.AddTranslation(Lang.Ru, "Размер");
            this.textSize.AddTranslation(Lang.Cn, "大小");

            this.textPosition = new MenuVectorSlider(settings, Hud.Info.ScanPosition + new Vector2(0, -50));

            // hidden alt key
            this.altKey = menu.Add(new MenuHoldKey("alt", Key.LeftAlt));
            this.altKey.Hide();
        }

        public void Activate()
        {
            this.LoadTextures();

            this.enabled.ValueChange += this.EnabledOnValueChange;

            maximumHealth = 6000 + GetBonusHealth(GameManager.GameTime);
            health = maximumHealth;
        }

        public void Dispose()
        {
            this.enabled.ValueChange -= this.EnabledOnValueChange;
            RendererManager.Draw -= this.OnDraw;
            //RendererManager.Draw -= this.OnDrawDrop;
            //this.showDrop.ValueChange -= this.ShowDropOnValueChange;
            this.printTime.ValueChange -= this.PrintTimeOnValueChange;
            GameManager.GameEvent -= this.OnGameEvent;
            UpdateManager.DestroyIngameUpdate(OnUpdate);
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            InputManager.MouseKeyUp -= this.InputManagerOnMouseKeyUp;
            this.textPosition.Dispose();
        }

        private void EnabledOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                RendererManager.Draw += this.OnDraw;
                //this.showDrop.ValueChange += this.ShowDropOnValueChange;
                this.printTime.ValueChange += this.PrintTimeOnValueChange;
                EntityManager9.UnitAdded += this.OnUnitAdded;
                GameManager.GameEvent += this.OnGameEvent;
                UpdateManager.CreateIngameUpdate(OnUpdate);
            }
            else
            {
                RendererManager.Draw -= this.OnDraw;
                //RendererManager.Draw -= this.OnDrawDrop;
                //this.showDrop.ValueChange -= this.ShowDropOnValueChange;
                InputManager.MouseKeyUp -= this.InputManagerOnMouseKeyUp;
                this.printTime.ValueChange -= this.PrintTimeOnValueChange;
                GameManager.GameEvent -= this.OnGameEvent;
                UpdateManager.DestroyIngameUpdate(OnUpdate);
                EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
                EntityManager9.UnitAdded -= this.OnUnitAdded;
            }
        }

        private void InputManagerOnMouseKeyUp(MouseEventArgs e)
        {
            try
            {
                if (e.MouseKey != MouseKey.Left || this.clickSleeper.IsSleeping)
                {
                    return;
                }

                var cd = GameManager.RawGameTime - this.roshanKillTime;
                if (cd > GameData.RoshanMaxRespawnTime)
                {
                    return;
                }

                var position = new Rectangle9(
                    new Vector2(this.textPosition.Value.X - this.textSize.Value, this.textPosition.Value.Y),
                    new Vector2(this.textSize.Value * 2));

                if (!position.Contains(e.Position))
                {
                    return;
                }

                var time = (GameData.RoshanMaxRespawnTime - cd) + GameManager.GameTime;
                var start = TimeSpan.FromSeconds(time - (GameData.RoshanMaxRespawnTime - GameData.RoshanMinRespawnTime)).ToString("mss");
                var end = TimeSpan.FromSeconds(time).ToString("mss");

                GameManager.ExecuteCommand("say_team \"" + start + " " + end + " rosh\"");
                this.clickSleeper.Sleep(30);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void LoadTextures()
        {
            RendererManager.LoadTexture("o9k.roshan_icon", @"panorama\images\hud\icon_roshan_psd.vtex_c");

            foreach (var ids in this.roshanDrop)
            {
                foreach (var id in ids)
                {
                    RendererManager.LoadTexture(id, AbilityTextureType.Round);
                }
            }
        }

        private void OnAbilityRemoved(Ability9 ability)
        {
            if (ability.Id != AbilityId.item_aegis || !ability.Owner.IsHero || ability.Owner.IsIllusion)
            {
                return;
            }

            this.aegisPickUpTime = float.MinValue;
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
        }

        private void OnDraw()
        {
            try
            {
                OnDrawDrop();

                var gameTime = GameManager.RawGameTime;
                var cd = gameTime - this.roshanKillTime;
                var aegisCd = gameTime - this.aegisPickUpTime;
                var showRemainingTime = this.showRemaining.IsEnabled;

                if (this.altKey)
                {
                    showRemainingTime = !showRemainingTime;
                }

                string text;

                if (cd > GameData.RoshanMaxRespawnTime)
                {
                    if (this.hide)
                    {
                        return;
                    }

                    text = "Alive";
                }
                else if (cd > GameData.RoshanMinRespawnTime)
                {
                    var time = GameData.RoshanMaxRespawnTime - cd;
                    if (!showRemainingTime)
                    {
                        time += GameManager.GameTime;
                    }

                    text = TimeSpan.FromSeconds(time).ToString(@"m\:ss") + "*";
                }
                else if (aegisCd <= GameData.AegisExpirationTime)
                {
                    var time = GameData.AegisExpirationTime - aegisCd;
                    if (!showRemainingTime)
                    {
                        time += GameManager.GameTime;
                    }

                    text = TimeSpan.FromSeconds(time).ToString(@"m\:ss") + "!";
                }
                else
                {
                    var time = GameData.RoshanMaxRespawnTime - cd;
                    if (!showRemainingTime)
                    {
                        time += GameManager.GameTime;
                    }

                    text = TimeSpan.FromSeconds(time).ToString(@"m\:ss");

                    if (this.showMinTime)
                    {
                        text = TimeSpan.FromSeconds(time - (GameData.RoshanMaxRespawnTime - GameData.RoshanMinRespawnTime))
                                   .ToString(@"m\:ss") + Environment.NewLine + text;
                    }
                }

                Drawer.DrawTextWithBackground(text, this.textSize, this.textPosition);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnDrawDrop()
        {
            if (this.showDrop || showSleeper)
            {
                var position = this.topPanel.GetTimePosition();
                position.Y += 100 * Hud.Info.ScreenRatio;
                position *= new Size2F(0.4f, 0.75f);

                var gameTime = GameManager.RawGameTime;

                var attacked = attackedSleeper.IsSleeping;
                var spawned = Roshan.PredictEntityIndex != -1;

                float scale;

                if (attacked)
                {
                    scale = (gameTime % 1) + 0.5f;
                    if (scale > 1)
                    {
                        scale = 2 - scale;
                    }
                }
                else
                {
                    scale = 1f;
                }

                var cd = gameTime - this.roshanKillTime;
                if (cd > GameData.RoshanMaxRespawnTime)
                {
                    if (spawned)
                    {
                        RendererManager.DrawTexture(attacked ? "o9k.outline_red" : "o9k.outline_green", position * scale * 1.15f);
                    }
                    else
                    {
                        RendererManager.DrawTexture("o9k.outline_yellow", position * 1.15f);
                    }
                }
                else
                {
                    RendererManager.DrawTexture(cd > GameData.RoshanMinRespawnTime ? "o9k.outline_green" : "o9k.outline_yellow", position * scale * 1.15f);

                    var pct = (int)((cd / GameData.RoshanMaxRespawnTime) * 100);
                    RendererManager.DrawTexture("o9k.outline_black" + pct, position * scale * 1.17f);
                }

                RendererManager.DrawTexture("o9k.roshan_icon", position * scale);

                var center = position.Center + new Vector2(0, position.Height * 0.65f);
                var items = this.roshan?.IsValid == true && this.roshan.BaseInventory != null
                                ? this.roshan.BaseInventory.MainItems.Select(x => x.Id).ToArray()
                                : this.roshanDrop[this.roshansKilled];
                var abilitiesSize = 20 * Hud.Info.ScreenRatio;
                var gap = 4 * Hud.Info.ScreenRatio;
                var start = new Vector2(center.X - ((abilitiesSize / 2f) * items.Length) - ((gap / 2f) * (items.Length - 1)), center.Y);

                for (var i = 0; i < items.Length; i++)
                {
                    RendererManager.DrawTexture(
                        items[i],
                        new Rectangle9(start + new Vector2((i * abilitiesSize) + (i * gap), 0), abilitiesSize, abilitiesSize),
                        AbilityTextureType.Round);
                }

                var hpWidth = 150 * Hud.Info.ScreenRatio;
                var hpCenter = position.Center + new Vector2(-(hpWidth / 2f), position.Height * 1.5f);
                var hpRect = new Rectangle9(hpCenter.X, hpCenter.Y, hpWidth, 24 * Hud.Info.ScreenRatio);

                var hp = spawned ? Math.Min(this.health + (GameManager.GameTime - lastTime) * 20, maximumHealth) : 0;
                RendererManager.DrawTexture("o9k.health_enemy_bg", hpRect);
                RendererManager.DrawTexture("o9k.health_enemy", new(hpRect.X, hpRect.Y, (hp / maximumHealth) * hpRect.Width, hpRect.Height), 0.7f);

                var text = $"{(int)hp}/{(int)maximumHealth}";
                var fontSize = 15 * Hud.Info.ScreenRatio;

                RendererManager.DrawText(text, hpRect + new Vector2(1, 1) * Hud.Info.ScreenRatio, Color.Black, FontFlags.Center | FontFlags.VerticalCenter, fontSize);
                RendererManager.DrawText(text, hpRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter, fontSize);
            }
        }

        private void OnGameEvent(GameEventEventArgs e)
        {
            var gameEvent = e.GameEvent;

            switch (gameEvent.Name)
            {
                case "dota_roshan_kill":
                    {
                        this.roshanKillTime = GameManager.RawGameTime;
                        this.roshan = null;

                        if (this.roshansKilled < this.roshanDrop.Length - 1)
                        {
                            this.roshansKilled++;
                        }

                        health = 0;
                    }
                    break;
                case "aegis_event":
                    {
                        this.aegisPickUpTime = GameManager.RawGameTime;
                        EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
                    }
                    break;
                case "entity_hurt":
                    {
                        var attackedIndex = gameEvent.GetInt32("entindex_killed");
                        if (attackedIndex != Roshan.PredictEntityIndex)
                        {
                            break;
                        }

                        health = (float)Math.Max(Math.Round(health - gameEvent.GetSingle("damage")), 0);
                        attackedSleeper.Sleep(5);
                        showSleeper.Sleep(10);
                    }
                    break;
            }
        }

        private void OnUpdate()
        {
            var time = GameManager.GameTime;

            if (Roshan.PredictEntityIndex != -1 && roshanKillTime != float.MinValue)
            {
                roshanKillTime = float.MinValue;

                health = 6000 + GetBonusHealth(time);
                lastTime = time;
            }

            if (roshan != null && roshan.IsValid && roshan.IsVisible)
            {
                maximumHealth = roshan.MaximumHealth;
                health = roshan.Health;
                lastTime = time;
                return;
            }

            if ((int)Math.Floor(time / 60) == (int)Math.Floor(lastTime / 60))
            {
                return;
            }

            maximumHealth = 6000 + GetBonusHealth(time);
            health = Math.Min(health + (time - lastTime) * 20, maximumHealth) * (maximumHealth / (6000 + GetBonusHealth(lastTime)));
            lastTime = time;
        }

        private int GetBonusHealth(float time)
        {
            var honusHealth = 115;
            if (GameManager.GameMode == GameMode.Turbo)
            {
                honusHealth *= 2;
            }

            return (int)Math.Floor(time / 60) * honusHealth;
        }

        private void OnUnitAdded(Unit9 unit)
        {
            try
            {
                if (unit is not Core.Entities.Units.Unique.Roshan)
                {
                    return;
                }

                this.roshan = unit;
                this.roshanKillTime = float.MinValue;

                maximumHealth = unit.MaximumHealth;
                health = unit.Health;
                lastTime = GameManager.GameTime;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void PrintTimeOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                InputManager.MouseKeyUp += this.InputManagerOnMouseKeyUp;
            }
            else
            {
                InputManager.MouseKeyUp -= this.InputManagerOnMouseKeyUp;
            }
        }

        //private void ShowDropOnValueChange(object sender, KeyEventArgs e)
        //{
        //    if (e.NewValue)
        //    {
        //        RendererManager.Draw += this.OnDrawDrop;
        //    }
        //    else
        //    {
        //        RendererManager.Draw -= this.OnDrawDrop;
        //    }
        //}
    }
}