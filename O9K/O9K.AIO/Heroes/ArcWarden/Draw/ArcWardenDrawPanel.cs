namespace O9K.AIO.Heroes.ArcWarden.Draw
{
    using System;
    using System.Collections.Generic;

    using Core.Managers.Menu.Items;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Extensions;
    using Divine.Input;
    using Divine.Input.EventArgs;
    using Divine.Numerics;
    using Divine.Renderer;

    using TargetManager;

    using Utils;

    public class AutoPushingPanelTest
    {
        public static Lane lane;

        private static int _optionsCount = Enum.GetNames(typeof(Lane)).Length;

        public static void OnMouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            for (int i = 0; i < _optionsCount; i++)
            {
                if (e.Position.IsUnderRectangle(new RectangleF(vector4PosClick[(Lane)i].X - 5, vector4PosClick[(Lane)i].Y - 5, vector4PosClick[(Lane)i].Z, vector4PosClick[(Lane)i].W)))
                {
                    lane = (Lane)i;
                }
            }
        }

        private static Dictionary<Lane, Vector4> vector4PosClick = new();

        public static Vector2 SizePanel = new();

        public static MenuSlider positionSliderX = new MenuSlider("position x", 0, 0, 2000);

        public static MenuSlider positionSliderY = new MenuSlider("position y", 600, 0, 2000);

        public static MenuSlider sizeMenuSlider = new MenuSlider("SIZE", 100, 100, 2000);

        public static void ButtonDrawOn()
        {
            var scaling = RendererManager.Scaling;
            var optionsCount = _optionsCount;
            var positionX = positionSliderX;
            var positionY = positionSliderY;
            var sizeMenuX = sizeMenuSlider;
            var sizeMenuY = sizeMenuX / 1.2f;
            var indent = sizeMenuX / 5;

            var rectBase = new RectangleF(
                x: positionX,
                y: positionY,
                width: ((int)(sizeMenuX + indent / 1.55f) * scaling) * optionsCount,
                height: sizeMenuX / 4f * scaling);

            var rectText = new RectangleF(
                x: rectBase.X,
                y: rectBase.Y - (10 * scaling),
                width: rectBase.Width,
                height: rectBase.Height + (10 * scaling));

            var rect = new RectangleF(
                x: rectBase.X,
                y: rectBase.Y + rectBase.Height + (3 * scaling),
                width: rectBase.Width,
                height: (sizeMenuY + indent) * scaling * 2f);

            RendererManager.DrawFilledRectangle(rect, Color.Black, new Color(0, 0, 0, 250), 2);
            SizePanel = (new Vector2(((sizeMenuX + indent / 1.55f) * scaling) * optionsCount, sizeMenuX / 4 * scaling));

            vector4PosClick.Clear();

            var fontSize = 35 * scaling;

        

            for (int i = 0; i < optionsCount; i++)
            {
                var posX = rect.X + (indent / 2 * scaling) + i * ((sizeMenuX + (indent / 2)) * scaling);
                var posY = rect.Y + (indent / 2 * scaling);
                var width = sizeMenuX * scaling;
                var height = sizeMenuY * scaling;
                vector4PosClick.Add((Lane)i, new Vector4(posX, posY, width, height));

                var rectBorderImage = new RectangleF(
                    x: posX,
                    y: posY,
                    width: width,
                    height: height);

                var rectText1 = new RectangleF(
                    x: posX + (10 * scaling),
                    y: posY + (10 * scaling),
                    width: width,
                    height: height);


                if ((Lane)i == lane)
                {
                    RendererManager.DrawRectangle(rectBorderImage, Color.Green);

                    RendererManager.DrawText(((Lane)i).ToString(), rectText1, Color.Green, fontSize);
                }
                else
                {
                    RendererManager.DrawRectangle(rectBorderImage, Color.Red);

                    RendererManager.DrawText(((Lane)i).ToString(), rectText1, Color.White, fontSize);
                }
            }
            
            var newRect = new RectangleF(rect.X + (rect.Width * 0.5f) - (rect.Width / 3) * 0.5f , rect.Y + (sizeMenuX + (indent * 0.5f)) * scaling, rect.Width / 3f, sizeMenuY * scaling);

            if (unitName != null)
            {
                RendererManager.DrawRectangle(newRect, Color.Black);

                RendererManager.DrawImage(unitName, newRect, UnitImageType.Default, true);
            }
            if (pushComboStatus)
            {
                RendererManager.DrawRectangle(newRect, Color.Black);

                RendererManager.DrawText("Push combo ACTIVE", new Vector2(rect.X  , rect.Y + (sizeMenuX + (indent * 0.5f)) * scaling), Color.Red, fontSize);
            }
            
        }

        public static string unitName { get; set; }

        public static bool pushComboStatus { get; set; }
    }
}