using System;

using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.Humanizer
{
    internal sealed class Screen
    {
        private static readonly float CameraAngleX = (float)Math.Sin(MathUtil.DegreesToRadians(CameraManager.Pitch));

        private static readonly Vector2 ScreenSize = RendererManager.ScreenSize;

        private static readonly float Ratio = ScreenSize.X / ScreenSize.Y;

        private Vector2 TopLeftTrigger;

        private Vector2 TopRightTrigger;

        private Vector2 BottomLeftTrigger;

        private Vector2 BottomRightTrigger;

        private readonly Humanizer Humanizer;

        private float TriggerValue;

        public Screen(Humanizer humanizer)
        {
            Humanizer = humanizer;

            humanizer.Menu.TriggerSwitcher.ValueChanged += (sender, e) =>
            {
                TriggerValue = Helper.Map(0f, 100f, 0.98f, 0.85f, e.NewValue);

                SetTriggers(Helper.Map(0f, 100f, 0.02f, 0.15f, e.NewValue));
            };

            UpdateManager.CreateIngameUpdate(OnUpdate);
            RendererManager.Draw += OnDraw;
        }

        private void OnDraw()
        {
            if (!Humanizer.Menu.DrawSwitcher)
            {
                return;
            }

            var rectSize = ScreenSize * 0.2f;
            var rect = new RectangleF(0, 489 - rectSize.Y, rectSize.X, rectSize.Y);

            RendererManager.DrawRectangle(rect, Color.Aqua, 3 * RendererManager.Scaling);

            var x = rect.Width - (rect.Width * TriggerValue);
            var y = rect.Height - (rect.Height * TriggerValue);

            RendererManager.DrawRectangle(new RectangleF(rect.X + x, rect.Y + (y * Ratio), rect.Width - (x * 2), rect.Height - (y * 2 * Ratio)), Color.Red, 3 * RendererManager.Scaling);
            RendererManager.DrawCircle(new Vector2(rect.X, rect.Y) + (Position * rectSize), 10, Color.Yellow, 3 * RendererManager.Scaling);

            RendererManager.DrawCircle(HUDInfo.WorldToMinimap(Humanizer.Camera.LookAt), 10, Color.Yellow, 3 * RendererManager.Scaling);

            /*if (Intersection != null)
            {
                RendererManager.DrawCircle(Intersection.Value * ScreenSize, 20, Color.Red, 5);
            }

            var topLeftTrigger = TopLeftTrigger * ScreenSize;
            var topRightTrigger = TopRightTrigger * ScreenSize;
            var bottomLeftTrigger = BottomLeftTrigger * ScreenSize;
            var bottomRightTrigger = BottomRightTrigger * ScreenSize;

            RendererManager.DrawLine(topLeftTrigger, topRightTrigger, Color.Red, 5);
            RendererManager.DrawLine(topRightTrigger, bottomRightTrigger, Color.Red, 5);
            RendererManager.DrawLine(bottomRightTrigger, bottomLeftTrigger, Color.Red, 5);
            RendererManager.DrawLine(bottomLeftTrigger, topLeftTrigger, Color.Red, 5);*/
        }

        private void OnUpdate()
        {
            var cameraPosition = Humanizer.Camera.Position.SetZ(CameraManager.Position.Z - CameraAngleX * CameraManager.Distance + CameraAngleX * CameraManager.DefaultDistance);

            var angle = new Vector3(CameraManager.DefaultPitch, CameraManager.DefaultYaw, CameraManager.DefaultRoll);
            if (CameraManager.WorldToScreen(MousePosition, cameraPosition, angle, CameraManager.DefaultDistance, ScreenSize, out var screenPosition))
            {
                Position = screenPosition;
                IsValid = true;

                var center = new Vector2(0.5f, 0.5f);

                Intersection = Helper.Intersection(TopLeftTrigger, TopRightTrigger, center, screenPosition)
                            ?? Helper.Intersection(TopRightTrigger, BottomRightTrigger, center, screenPosition)
                            ?? Helper.Intersection(BottomRightTrigger, BottomLeftTrigger, center, screenPosition)
                            ?? Helper.Intersection(BottomLeftTrigger, TopLeftTrigger, center, screenPosition);

                Humanizer.Camera.TEST = Intersection != null;
            }
            else
            {
                Position = Vector2.Zero;
                IsValid = false;

                Humanizer.Camera.TEST = true;
            }
        }

        public Vector3 MousePosition { private get; set; }

        public Vector2 Position { get; private set; }

        public bool IsValid { get; private set; }

        public Vector2? Intersection { get; private set; }

        private void SetTriggers(float value)
        {
            TopLeftTrigger = new Vector2(value, value * Ratio);
            TopRightTrigger = new Vector2(1 - value, value * Ratio);
            BottomLeftTrigger = new Vector2(value, 1 - (value * Ratio));
            BottomRightTrigger = new Vector2(1 - value, 1 - (value * Ratio));
        }
    }
}