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
using MessageBox = System.Windows.MessageBox;
using System.Collections.Concurrent;

namespace Divine.Plugin
{
    internal sealed class Bootstrap : Bootstrapper
    {
        protected override void OnActivate()
        {
            LogManager.Info("Activate Divine.Plugin");
        }

        private MenuHoldKey HoldKey;

        protected override void OnPreActivate()
        {
            LogManager.Info("PreActivate Divine.Plugin");

            SystemManager.UnhandledException += OnUnhandledException;

            GameManager.ExecuteCommand("dota_new_player false");

            //RendererManager.LoadTextureFromAssembly("Divine.Plugin.Resources.aa.png", "Divine.Plugin.Resources.aa.png", new TextureProperties() { ColorRatio = new Vector4(0, 0, 0, 1) });

            //InputManager.WndProc += InputManager_WndProc;
            //InputManager.WndProc += InputManager_WndProcBBB;

            //RendererManager.Draw += OnRendererManagerDraw;
            //Entity.NetworkPropertyChanged += Entity_NetworkPropertyChanged;
            //Entity.AnimationChanged += Entity_AnimationChanged;
            //ParticleManager.ParticleAdded += ParticleManager_ParticleAdded;
            //ParticleManager.ParticleRemoved += ParticleManager_ParticleRemoved;
            //ModifierManager.ModifierAdded += OnModifierManagerModifierAdded;
            //ModifierManager.ModifierRemoved += OnModifierManagerModifierRemoved;
            //OrderManager.OrderAdding += OrderManager_OrderAdding;
            //OrderManager.OrderOverwatchAdding += OrderOverwatchAdding;
            //ProjectileManager.TrackingProjectileAdded += ProjectileManager_TrackingProjectileAdded;
            //ProjectileManager.TrackingProjectileRemoved += ProjectileManager_TrackingProjectileRemoved;
            //ProjectileManager.LinearProjectileAdded += ProjectileManager_LinearProjectileAdded;
            //ProjectileManager.LinearProjectileRemoved += ProjectileManager_LinearProjectileRemoved;
            //GameManager.GameEvent += GameManagerGameEvent;

            //EntityManager.EntityAdded += EntityManager_EntityAdded;

            //UpdateManager.Update += OnUpdate;
            //UpdateManager.GameUpdate += OnGameUpdate;

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

            /*foreach (var item in EntityManager.GetEntities<Tree>())
            {
                Console.WriteLine(item.Model);
                item.Model = "models/props_stone/stone_column002a.vmdl_c";
            }*/

            //var localHero = EntityManager.LocalHero;
            //localHero.Glow = new Color(255, 255, 0, 0);

        }

        private void GameManagerGameEvent(GameEventEventArgs e)
        {
            var gameEvent = e.GameEvent;
            if (gameEvent.Name == "entity_hurt")
            {
                Console.WriteLine(gameEvent.GetSingle("damage"));
            }
            

            /*Console.WriteLine(e.Name);

            foreach (var item in e.KeyValue.KeyValues)
            {
                Console.WriteLine(item.Name + "  " + item.GetUInt64());
            }

            Console.WriteLine(EntityManager.LocalHero.Index);


            Console.WriteLine();*/
        }

        private void EntityManager_EntityAdded(EntityAddedEventArgs e)
        {
            Console.WriteLine("EntityManager_EntityAdded " + GameManager.Time);
            UpdateManager.BeginInvoke(() =>
            {
                Console.WriteLine("EntityManager_EntityAdded+++ " + e.Entity.Team + "  " + GameManager.Time);
            });
        }

        private void OrderOverwatchAdding(OrderOverwatchAddingEventArgs e)
        {
            Console.WriteLine(e.OrderOverwatch.MousePosition + "  " + e.OrderOverwatch.CameraPosition);
        }

        private void ParticleManager_ParticleAdded(ParticleAddedEventArgs e)
        {
            if (e.IsCollection)
            {
                return;
            }

            Console.WriteLine(e.Particle.Handle + "  " + e.Particle.Index);
        }

        private void ProjectileManager_TrackingProjectileAdded(TrackingProjectileAddedEventArgs e)
        {
            var trackingProjectile = e.TrackingProjectile;
            Console.WriteLine("TrackingProjectileAdded: " + trackingProjectile.Source + "  " + trackingProjectile.Target + "  " + trackingProjectile.TargetPosition + "  " + trackingProjectile.Handle);
        }

        private void ProjectileManager_TrackingProjectileRemoved(TrackingProjectileRemovedEventArgs e)
        {
            var trackingProjectile = e.TrackingProjectile;
            Console.WriteLine("TrackingProjectileRemoved: " + trackingProjectile.Source + "  " + trackingProjectile.Target + "  " + trackingProjectile.Position + "  " + trackingProjectile.Handle);
        }

        private void ProjectileManager_LinearProjectileAdded(LinearProjectileAddedEventArgs e)
        {
            var linearProjectile = e.LinearProjectile;
            //Console.WriteLine("LinearAdded: " + linearProjectile.StartPosition + "  " + linearProjectile.Position + "  " + linearProjectile.Handle);
        }

        private void ProjectileManager_LinearProjectileRemoved(LinearProjectileRemovedEventArgs e)
        {
            var linearProjectile = e.LinearProjectile;
            //Console.WriteLine("LinearRemoved: " + linearProjectile.StartPosition + "  " + linearProjectile.Position + "  " + linearProjectile.Handle);
        }

        private void GameManager_GameUpdate()
        {

        }

        private void Entity_NetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "m_angRotation" && sender.ClassId == ClassId.CDOTA_Unit_Hero_Mirana)
            {
                Console.WriteLine(e.PropertyName + "  " + ((Unit)sender).Rotation);
            }

            if (e.PropertyName != "m_anglediff" || sender.ClassId != ClassId.CDOTA_Unit_Hero_Mirana)
            {
                return;
            }

            Console.WriteLine(e.PropertyName + "  " + ((Unit)sender).Rotation + "  " + e.NewValue.GetInt32());

            /*UpdateManager.BeginInvoke(() =>
            {
                //Console.WriteLine(sender + "  " + (NetworkActivity)e.OldValue.GetInt32());
                Console.WriteLine(sender + "  " + e.PropertyName + "  " + e.NewValue.GetInt32());
            });*/
        }

        private void OnRendererManagerDraw()
        {
            /*var fff = new TextureProperties()
            {
                ColorTint = new Color(255, 1, 1, 255)
            };

            RendererManager.LoadTexture("GFFFFFFFFFFFFFFF", "panorama/images/hud/dota_psd.vtex_c", fff);

            Console.WriteLine(fff.ColorRatio + "  " + fff.ColorTint);

            RendererManager.DrawTexture("GFFFFFFFFFFFFFFF", new(500, 500, 500, 500));*/
        }

        private void OrderManager_OrderAdding(OrderAddingEventArgs e)
        {
            Console.WriteLine(e.Order.Type + "  " + e.Order.TargetIndex + "  " + e.Order.AbilityIndex);
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

        private unsafe void OnUpdate()
        {
            //Console.WriteLine("OnUpdate: " + GameManager.Time);

            var localHero = EntityManager.LocalHero;
            if (localHero == null)
            {
                return;
            }

            if (GameManager.IsPaused)
            {
                return;
            }

            if (GameManager.RawGameTime - Time > 0.005f)
            {
                var hero = EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsAlly(localHero));
                //Console.WriteLine(localHero.Spellbook.Spell2.IsInAbilityPhase  + "  " + localHero.Rotation + "  " + localHero.RotationDifference);

                //Player.Cast(localHero, creeps, Game.MousePosition, Vector3.Zero);
                Time = GameManager.RawGameTime;
                /*var hero = EntityManager.GetEntities<Hero>().OrderBy(x => x.Distance2D(localHero)).FirstOrDefault(x => x.IsAlly(localHero) && x != localHero);
                OrderManager.CreateOrder(OrderType.MovePosition, localHero, 0, 0, hero.Position, false, true, false);*/

                //Console.WriteLine(localHero.Spellbook.Spell2.CastPoint);

                //ParticleManager.LineParticle("Ytyty", localHero.Position, localHero.InFront(1000), 50, Color.Red);

                //OrderManager.CreateOrder(OrderType.MovePosition, bomb, 0, 879, Vector3.Zero, false, false, false);

                /*var sw = Stopwatch.StartNew();
                for (int i = 0; i < 10000000; i++)
                {
                    var fff = EntityManager.GetPlayerById(6);
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed);*/
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

        private void OnGameUpdate()
        {
            Console.WriteLine("OnGameUpdate: " + GameManager.Time);
        }
    }
}