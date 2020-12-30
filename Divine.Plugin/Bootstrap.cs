using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

using Divine.Menu;
using Divine.Menu.Items;
using Divine.SDK.Managers.Update;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;

namespace Divine.Plugin
{
    internal sealed class Bootstrap : Bootstrapper
    {
        protected override void OnActivate()
        {
            //new DotaMap.Bootstrap();
        }

        private MenuHoldKey HoldKey;

        protected unsafe override void OnPreActivate()
        {
            Console.WriteLine("======================================================================================");
            Console.WriteLine("Test in Plugin: " + typeof(Bootstrap).FullName);
            Console.WriteLine("======================================================================================");

            GameManager.UnhandledException += Game_UnhandledException;

            //InputManager.WndProc += InputManager_WndProc;
            //InputManager.WndProc += InputManager_WndProcBBB;

            var fff = MenuManager.CreateRootMenu("TEST");
            var ttt = fff.CreateMenu("TEST2");
            ttt.CreateText("TEST").SetFontColor(Color.Red);
            fff.CreateSlider("Slider", 0, 0, 100);
            fff.SetTooltip("O YEEEEE");
            HoldKey = fff.CreateHoldKey("HoldKey", Key.D);

            //Game.FireEvent += Game_FireEvent;
            //Game.GameStateChanged += Game_GameStateChanged;
            //RendererManager.D3D11Renderer.Draw += OnD3D11RendererDraw;
            //RendererManager.GameRenderer.Draw += OnGameRendererDraw;
            //Entity.NetworkPropertyChanged += Entity_NetworkPropertyChanged;
            //Entity.AnimationChanged += Entity_AnimationChanged;
            //ParticleManager.ParticleAdded += ParticleManager_ParticleAdded;
            //ParticleManager.ParticleRemoved += ParticleManager_ParticleRemoved;
            //ModifierManager.ModifierAdded += OnModifierManagerModifierAdded;
            //ModifierManager.ModifierRemoved += OnModifierManagerModifierRemoved;

            //EntityManager.EntityAdded += EntityManager_EntityAdded;

            GameManager.Update += OnGameUpdate;
        }

        private void EntityManager_EntityAdded(EntityAddedEventArgs e)
        {
            Console.WriteLine("EntityAdded: " + GameManager.Time);
        }

        private void OnModifierManagerModifierAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            Console.WriteLine("ModifierAdded: " + modifier.Name + " | " + modifier.Owner + " | " + modifier.Caster);
        }
        private void OnModifierManagerModifierRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            Console.WriteLine("ModifierRemoved: " + modifier.Name + " | " + modifier.Owner + " | " + modifier.Caster);
        }

        private void ParticleManager_ParticleAdded(ParticleAddedEventArgs e)
        {
            var particle = e.Particle;
            if (!particle.Name.Contains("generic_hit_blood"))
            {
                //return;
            }
        }

        private void ParticleManager_ParticleRemoved(ParticleRemovedEventArgs e)
        {
            //Console.WriteLine("ParticleRemoved: " + e.Particle.Position + " | " + e.Particle.Name);
        }

        private void Entity_AnimationChanged(Entity sender, AnimationChangedEventArgs e)
        {
            Console.WriteLine(sender + "  " + e.Name);
        }

        private void Game_GameStateChanged(GameStateChangedEventArgs e)
        {
            Console.WriteLine(e.NewGameState + "  " + e.OldGameState);
        }

        private void Game_FireEvent(FireEventEventArgs e)
        {
        }

        private unsafe void Entity_NetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "m_iNetTimeOfDay")
            {
                return;
            }

            /*if (e.NewValue.GetEntity() == null)
            {
                return;
            }*/

            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            //Console.WriteLine(sender + "  " + e.ValueTypeName + "  " + e.PropertyName + "  " + e.NewValue.GetInt32() + "  " + e.OldValue.GetInt32());
        }

        private void Game_UnhandledException(Exception e)
        {
            Console.WriteLine(e);
        }

        protected override void OnDeactivate()
        {
        }

        private Particle Particle;

        private float Time;

        private unsafe void OnGameUpdate()
        {
            var localHero = EntityManager.LocalHero;
            if (localHero == null)
            {
                return;
            }

            return;
            /*var particleNmee = "materials/ensage_ui/particles/range_display_mod.vpcf";
            var controlPoints = new[] { new ControlPoint(0, Game.MousePosition), new ControlPoint(1, 255, 255, 5), new ControlPoint(2, 255, 255, 255) };

            ParticleManager.CreateOrUpdateParticle("TEST", particleNmee, localHero, ParticleAttachment.AbsOrigin, controlPoints);*/

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1; i++)
            {
            }
            sw.Stop();
            Console.WriteLine("AAAAA: " + sw.Elapsed);

            if (GameManager.RawGameTime - Time > 1f)
            {
                //Player.Cast(localHero, creeps, Game.MousePosition, Vector3.Zero);
                Time = GameManager.RawGameTime;

                /*foreach (var item in ModifierManager.Modifiers)
                {
                    Console.WriteLine(item.Owner + " | " + item.Caster + " | " + item.Name);
                }

                Console.WriteLine();*/

            }

            /*if (Particle == null)
            {
                Particle = ParticleManager.CreateParticle(@"materials\\ensage_ui\\particles\\target.vpcf", ParticleAttachment.AbsOrigin, localHero);
            }

            var random = new Random();

            Particle.SetControlPoint(0, Game.MousePosition);
            Particle.SetControlPoint(1, new Vector3(random.Next(100, 500), 255, 5));
            Particle.SetControlPoint(2, new Vector3(255, 255, 255));*/

            //var fff = ParticleManager.Particles;

            for (var i = 0; i <= 10; i++)
            {
                //Console.WriteLine(Particle.GetControlPoint(i));
            }

            //Console.WriteLine(Particle.HighestControlPoint);
            //Console.WriteLine(Particle.IsInFogVisible);

            /*if (localHero.IsVisibleToEnemies)
            {
                if (Particle == null)
                {
                    Particle = ParticleManager.CreateParticle(@"materials\ensage_ui\particles\range_display_mod.vpcf", ParticleAttachment.AbsOriginFollow, localHero);
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