using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

using Divine.Extensions;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.SDK.Extensions;
using Divine.SDK.Managers.Log;

using Newtonsoft.Json.Linq;

using SharpDX;
using SharpDX.Mathematics.Interop;

using Divine.Zero.Sandbox;
using Divine.Items;
using Divine.SDK.Orbwalker;
using Divine.SDK.Helpers;
using System.Security.Cryptography;
using Divine.SDK.Prediction;
using Divine.SDK.Prediction.Collision;
using Divine.SDK.Localization;

namespace Divine.Plugin
{
    internal sealed class Bootstrap : Bootstrapper
    {
        private readonly Sleeper Sleeper = new();

        protected override void OnActivate()
        {
            Console.WriteLine("OnActivate");

            var humanizer = new Humanizer.Humanizer();
            humanizer.OnActivate();

            Console.WriteLine("Divine.Plugin");
            //new TogetherWeStand(MenuManager.CreateRootMenu("TogetherWeStand"));
        }

        private MenuHoldKey HoldKey;

        protected override void OnPreActivate()
        {
            Console.WriteLine("======================================================================================");
            Console.WriteLine("Test in Plugin: " + typeof(Bootstrap).FullName);
            Console.WriteLine("======================================================================================");

            SystemManager.UnhandledException += OnUnhandledException;

            //InputManager.WndProc += InputManager_WndProc;
            //InputManager.WndProc += InputManager_WndProcBBB;

            //Game.FireEvent += Game_FireEvent;
            //Game.GameStateChanged += Game_GameStateChanged;
            //RendererManager.Draw += OnRendererManagerDraw;
            //Entity.NetworkPropertyChanged += Entity_NetworkPropertyChanged;
            //Entity.AnimationChanged += Entity_AnimationChanged;
            //ParticleManager.ParticleAdded += ParticleManager_ParticleAdded;
            //ParticleManager.ParticleRemoved += ParticleManager_ParticleRemoved;
            //ModifierManager.ModifierAdded += OnModifierManagerModifierAdded;
            //ModifierManager.ModifierRemoved += OnModifierManagerModifierRemoved;
            //OrderManager.OrderAdding += OrderManager_OrderAdding;
            //OrderManager.OrderLiveGameAdding += OrderManager_OrderLiveGameAdding;

            //EntityManager.EntityAdded += EntityManager_EntityAdded;

            UpdateManager.IngameUpdate += OnGameUpdate;

            var cameraRootMenu = MenuManager.CreateRootMenu("Camera");
            cameraRootMenu.CreateSlider("Pitch", 60, 0, 359).ValueChanged += async (slider, e) =>
            {
                CameraManager.Pitch = e.NewValue;

                await Task.Delay(50);
            };

            cameraRootMenu.CreateSlider("Yaw", 90, 0, 359).ValueChanged += async (slider, e) =>
            {
                CameraManager.Yaw = e.NewValue;

                await Task.Delay(50);
            };

            //RendererManager.Draw += GameManager_GameUpdate;
        }

        private void GameManager_GameUpdate()
        {
            /*var screenPosition = GameManager.MouseScreenPosition;
            RendererManager.DrawText("A", screenPosition - new Vector2(0, 100), Color.Red, 100);
            RendererManager.DrawText(
                "A",
                new RectangleF(screenPosition.X - (ushort.MaxValue / 2), screenPosition.Y - (ushort.MaxValue / 2), ushort.MaxValue, ushort.MaxValue),
                Color.Blue,
                "Tahoma",
                FontFlags.Center | FontFlags.VerticalCenter,
                100);*/

            
            /*RendererManager.DrawTexture("panorama/images/backgrounds/sidelane_jpg.vtex_c", new RectangleF(0, 200, 300, 300), TextureType.Default, true);
            RendererManager.DrawTexture("panorama/images/backgrounds/sidelane_jpg.vtex_c", new RectangleF(300, 200, 300, 300), TextureType.Round, true);
            RendererManager.DrawTexture("panorama/images/backgrounds/sidelane_jpg.vtex_c", new RectangleF(600, 200, 300, 300), TextureType.Square, true);

            RendererManager.LoadTexture("A1", "panorama/images/backgrounds/sidelane_jpg.vtex_c", new TextureProperties { IsBlackWhite = true });
            RendererManager.DrawTexture("A1", new RectangleF(900, 200, 300, 300));

            RendererManager.LoadTexture("A2", "panorama/images/backgrounds/sidelane_jpg.vtex_c", new TextureProperties { Brightness = 100 });
            RendererManager.DrawTexture("A2", new RectangleF(1200, 200, 300, 300));

            RendererManager.LoadTexture("A3", "panorama/images/backgrounds/sidelane_jpg.vtex_c", new TextureProperties { ColorRatio = new Vector4(5, 5, 5, 5) });
            RendererManager.DrawTexture("A3", new RectangleF(1500, 200, 300, 300));*/
        }

        private void Entity_NetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "m_NetworkActivity")
            {
                return;
            }

            Console.WriteLine((NetworkActivity)e.NewValue.GetInt32());
        }

        private void OnRendererManagerDraw()
        {
            MapMeshDrawer.Draw();
        }

        private void OrderManager_OrderAdding(OrderAddingEventArgs e)
        {
            Console.WriteLine(e.Order.Position);
            var localHero = EntityManager.LocalHero;
            //e.Order.Units = EntityManager.GetEntities<Unit>().Where(x => x.Team == localHero.Team);
            //e.Order.Position = new Vector3();
        }

        private void OnUnhandledException(Exception e)
        {
            LogManager.Error(e);
        }

        protected override void OnDeactivate()
        {

        }

        private Particle Particle;

        private float Time;

        public static Vector3 InFront(Unit unit, float distance)
        {
            var alpha = unit.RotationRad;
            var vector2FromPolarAngle = SharpDXExtensions.FromPolarCoordinates(1f, alpha);

            var v = unit.Position + (vector2FromPolarAngle.ToVector3() * distance);
            return new Vector3(v.X, v.Y, 0);
        }

        private unsafe void OnGameUpdate()
        {
            var localHero = EntityManager.LocalHero;
            if (localHero == null)
            {
                return;
            }

            //var ff = LocalizationHelper.LocalizeName(AbilityId.item_arcane_blink);

            //Console.WriteLine(ff); 

            /*var particleNmee = "materials/ensage_ui/particles/range_display_mod.vpcf";
            var controlPoints = new[] { new ControlPoint(0, Game.MousePosition), new ControlPoint(1, 255, 255, 5), new ControlPoint(2, 255, 255, 255) };

            ParticleManager.CreateOrUpdateParticle("TEST", particleNmee, localHero, ParticleAttachment.AbsOrigin, controlPoints)h;*/

            if (GameManager.RawGameTime - Time > 2f)
            {
                //Player.Cast(localHero, creeps, Game.MousePosition, Vector3.Zero);
                Time = GameManager.RawGameTime;
                /*var hero = EntityManager.GetEntities<Hero>().OrderBy(x => x.Distance2D(localHero)).FirstOrDefault(x => x.IsAlly(localHero) && x != localHero);
                OrderManager.CreateOrder(OrderType.MovePosition, localHero, 0, 0, hero.Position, false, true, false);*/


                /*var s = localHero.Spellbook.Spell2;

                var sw = Stopwatch.StartNew();
                for (int i = 0; i < 1; i++)
                {
                    var ttt = s.CastPoint;
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed);*/

                /*foreach (var item in EntityManager.GetEntities<PowerTreads>())
                {
                    Console.WriteLine(item.ActiveAttribute);
                }*/
            }


            /*if (localHero.IsVisibleToEnemies)
            {
                if (Particle == null)
                {
                    Particle = ParticleManager.CreateParticle("particles/items_fx/aura_shivas.vpcf", ParticleAttachment.AbsOriginFollow, localHero);
                }
            }
            else if (Particle != null)
            {
                Particle.Destroy();
                Particle = null;
            }*/
        }
    }
}