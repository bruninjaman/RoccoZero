namespace O9K.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Divine;

    using Logger;

    using SharpDX;

    public static class Hud
    {
        private static readonly Dictionary<string, float> Messages = new Dictionary<string, float>();

        public static Vector3 CameraPosition
        {
            get
            {
                return CameraManager.LookAtPosition;
            }
            set
            {
                CameraManager.LookAtPosition = value;
            }
        }

        public static void CenterCameraOnHero(bool enabled = true)
        {
            GameManager.ExecuteCommand((enabled ? "+" : "-") + "dota_camera_center_on_hero");
        }

        public static void DisplayWarning(string text, float time = 10)
        {
            try
            {
                if (Messages.ContainsKey(text))
                {
                    Messages[text] = GameManager.RawGameTime + time;
                    return;
                }

                Messages.Add(text, GameManager.RawGameTime + time);

                if (Messages.Count == 1)
                {
                    RendererManager.Draw += OnDraw;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static bool IsPositionOnScreen(Vector3 position)
        {
            /*if (position.Z == 0)
            {
                //todo get proper Z

                if (Drawing.WorldToScreen(position.SetZ(128), out _) && Drawing.WorldToScreen(position.SetZ(384), out _))
                {
                    return true;
                }
            }
            else
            {
                if (Drawing.WorldToScreen(position, out _))
                {
                    return true;
                }
            }*/

            return true;
        }

        private static void OnDraw()
        {
            try
            {
                if (Messages.Count == 0)
                {
                    RendererManager.Draw -= OnDraw;
                }

                var position = new Vector2(Info.ScreenSize.X * 0.13f, Info.ScreenSize.Y * 0.05f);

                foreach (var message in Messages.ToList())
                {
                    var text = message.Key;
                    var time = message.Value;

                    if (GameManager.RawGameTime > time)
                    {
                        Messages.Remove(text);
                        continue;
                    }

                    position += new Vector2(0, 35);

                    RendererManager.DrawText(text, position, Color.OrangeRed, 33 * Info.ScreenRatio);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public static class Info
        {
            public static Vector2 GlyphPosition { get; } = RendererManager.ScreenSize * new Vector2(0.16f, 0.965f);

            public static Vector2 ScanPosition { get; } = RendererManager.ScreenSize * new Vector2(0.16f, 0.925f);

            public static float ScreenRatio { get; } = RendererManager.ScreenSize.Y / 1080f;

            public static Vector2 ScreenSize { get; } = RendererManager.ScreenSize;
        }
    }
}