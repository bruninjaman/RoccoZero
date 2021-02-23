//TODO
//namespace O9K.Hud.Modules.Screen.Time
//{
//    using System;
//    using System.Linq;
//    using System.Windows.Input;

//    using Core.Entities.Heroes;
//    using Core.Helpers;
//    using Core.Logger;
//    using Core.Managers.Entity;
//    using Core.Managers.Menu;
//    using Core.Managers.Menu.Items;

//    using Divine;
//    using Divine.SDK.Managers.Update;

//    using Helpers;

//    using MainMenu;

//    using SharpDX;

//    internal class MoveTime : IHudModule
//    {
//        private readonly MenuHoldKey key;

//        private readonly MenuVectorSlider textPosition;

//        private readonly MenuSlider textSize;

//        private int moveTime;

//        private Owner owner;

//        private NavMeshPathfinding pathfinder;

//        private UpdateHandler updateHandler;

//        public MoveTime(IHudMenu hudMenu)
//        {
//            var timeMenu = hudMenu.ScreenMenu.GetOrAdd(new Menu("Time"));
//            timeMenu.AddTranslation(Lang.Ru, "Время");
//            timeMenu.AddTranslation(Lang.Cn, "时间");

//            var menu = timeMenu.Add(new Menu("Move time"));
//            menu.AddTranslation(Lang.Ru, "Время движения");
//            menu.AddTranslation(Lang.Cn, "移动时间");

//            this.key = menu.Add(
//                new MenuHoldKey("Key", Key.LeftAlt).SetTooltip("Show approximate hero move time to mouse cursor's position"));
//            this.key.AddTranslation(Lang.Ru, "Клавиша");
//            this.key.AddTooltipTranslation(Lang.Ru, "Показать преблизительное время движения героя до курсора мыши");
//            this.key.AddTranslation(Lang.Cn, "键");
//            this.key.AddTooltipTranslation(Lang.Cn, "显示英雄移动到鼠标光标位置的大概时间");

//            var settings = menu.Add(new Menu("Settings"));
//            settings.AddTranslation(Lang.Ru, "Настройки");
//            settings.AddTranslation(Lang.Cn, "设置");

//            this.textSize = settings.Add(new MenuSlider("Size", 17, 10, 35));
//            this.textSize.AddTranslation(Lang.Ru, "Размер");
//            this.textSize.AddTranslation(Lang.Cn, "大小");

//            this.textPosition = new MenuVectorSlider(
//                settings,
//                new Vector3(34 * Hud.Info.ScreenRatio, -300, 300),
//                new Vector3(10 * Hud.Info.ScreenRatio, -300, 300));
//        }

//        public void Activate()
//        {
//            this.owner = EntityManager9.Owner;

//            /*RendererManager.TextureManager.LoadFromDota(
//                "o9k.waypoint_white",
//                @"panorama\images\hud\reborn\ping_icon_waypoint_psd.vtex_c",
//                new TextureProperties
//                {
//                    ColorRatio = new Vector4(1f, 1f, 1f, 1f)
//                });*/

//            this.pathfinder = new NavMeshPathfinding();
//            this.updateHandler = UpdateManager.CreateIngameUpdate(300, false, this.OnUpdate);
//            this.key.ValueChange += this.KeyOnValueChange;
//        }

//        public void Dispose()
//        {
//            this.pathfinder.Dispose();
//            this.textPosition.Dispose();
//            this.pathfinder.Dispose();
//            this.key.ValueChange -= this.KeyOnValueChange;
//            RendererManager.Draw -= this.OnDraw;
//            UpdateManager.DestroyIngameUpdate(this.updateHandler);
//        }

//        private void KeyOnValueChange(object sender, Core.Managers.Menu.EventArgs.KeyEventArgs e)
//        {
//            if (e.NewValue)
//            {
//                this.pathfinder.UpdateNavMesh();
//                this.updateHandler.IsEnabled = true;
//                RendererManager.Draw += this.OnDraw;
//            }
//            else
//            {
//                RendererManager.Draw -= this.OnDraw;
//                this.updateHandler.IsEnabled = false;
//                this.moveTime = 0;
//            }
//        }

//        private void OnDraw()
//        {
//            try
//            {
//                if (this.moveTime <= 0)
//                {
//                    return;
//                }

//                var position = GameManager.MouseScreenPosition + this.textPosition;
//                var size = this.textSize * Hud.Info.ScreenRatio;
//                var textureSize = new Vector2(size * 1.8f);

//                position -= new Vector2(0, textureSize.Y * 0.15f);
//                RendererManager.DrawTexture("o9k.waypoint_white", new RectangleF(position.X, position.Y, textureSize.X, textureSize.Y));
//                RendererManager.DrawText(this.moveTime + "s", position + new Vector2(textureSize.X, 0), Color.White, size);
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }

//        private void OnUpdate()
//        {
//            try
//            {
//                var hero = this.owner.Hero;
//                if (hero.Speed <= 0)
//                {
//                    this.moveTime = 0;
//                    return;
//                }

//                var position = hero.Position;
//                var path = this.pathfinder.CalculateStaticLongPath(position, GameManager.MousePosition, 999999, true, out var completed).ToArray();

//                if (!completed || path.Length == 0)
//                {
//                    return;
//                }

//                var time = 0f;

//                foreach (var vector3 in path)
//                {
//                    time += position.Distance2D(vector3) / hero.Speed;
//                    position = vector3;
//                }

//                this.moveTime = (int)Math.Ceiling(time + hero.GetTurnTime(path.Last()) + (GameManager.Ping / 1000f));
//            }
//            catch (Exception e)
//            {
//                Logger.Error(e);
//            }
//        }
//    }
//}