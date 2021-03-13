using System;

using Divine.SDK.Extensions;
using Divine.SDK.Helpers;

using SharpDX;

namespace Divine.Humanizer
{
    internal sealed class Humanizer
    {
        public Menu Menu;

        public Camera Camera;

        public Screen Screen;

        private readonly Sleeper CustomMousePositionSleeper = new Sleeper();

        private Vector3 CustomMousePosition;

        public void OnActivate()
        {
            Menu = new Menu();
            Camera = new Camera(this);
            Screen = new Screen(this);

            OrderManager.OrderAdding += OnOrderAdding;
            OrderManager.OrderOverwatchAdding += OnOrderOverwatchAdding;

            var random = new Random();

            UpdateManager.CreateIngameUpdate(1000, () =>
            {
                if (Menu.TestSwitcher)
                {
                    EntityManager.LocalHero.Move(new Vector3(random.Next(-5000, 5000), random.Next(-5000, 5000), 0));
                }
            });
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            if (!e.Process)
            {
                return;
            }

            if (!e.IsCustom)
            {
                CustomMousePositionSleeper.Reset();
            }
            else
            {
                var order = e.Order;
                switch (order.Type)
                {
                    case OrderType.MovePosition:
                    case OrderType.AttackPosition:
                    case OrderType.CastPosition:
                    case OrderType.MoveToDirection:
                        {
                            CustomMousePositionSleeper.Sleep(10000);
                            CustomMousePosition = order.Position;
                        }
                        break;

                    case OrderType.MoveTarget:
                    case OrderType.AttackTarget:
                    case OrderType.CastTarget:
                    case OrderType.CastTree:
                    case OrderType.CastRune:
                        {
                            CustomMousePositionSleeper.Sleep(10000);
                            CustomMousePosition = order.Target.Position;
                        }
                        break;
                }
            }
        }

        private void OnOrderOverwatchAdding(OrderOverwatchAddingEventArgs e)
        {
            var orderOverwatch = e.OrderOverwatch;
            var mousePosition = CustomMousePositionSleeper.Sleeping ? CustomMousePosition : orderOverwatch.MousePosition;

            Camera.MousePosition = mousePosition;
            Screen.MousePosition = mousePosition;

            var angles = new Vector3(CameraManager.DefaultPitch, CameraManager.DefaultYaw, CameraManager.DefaultRoll);
            if (CameraManager.WorldToScreen(mousePosition, CameraManager.CalculatePosition(Camera.Position, angles, CameraManager.DefaultDistance), angles, CameraManager.DefaultDistance, RendererManager.ScreenSize, out var screenPosition))
            {
                orderOverwatch.MouseScreenPositionPercent = screenPosition;
            }

            orderOverwatch.CameraPosition = Camera.Position.ToVector2();
        }
    }
}