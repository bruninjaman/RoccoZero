namespace O9K.AIO.Heroes.ArcWarden.Draw
{
    using System;
    using System.Collections.Generic;

    using Core.Managers.Menu.Items;

    using Divine.Extensions;
    using Divine.Input;
    using Divine.Input.EventArgs;
    using Divine.Numerics;
    using Divine.Renderer;

    using Utils;

    public class ArcWardenDrawPanel
    {
        public static Lane lane;

        private static readonly int _optionsCount = Enum.GetNames(typeof(Lane)).Length;

        private static readonly Dictionary<Lane, Vector4> vector4PosClick = new();

        public static Vector2 SizePanel  ;

        public static MenuSlider positionSliderX = new("position x", 0, 0, 2000);

        public static MenuSlider positionSliderY = new("position y", 600, 0, 2000);

        public static MenuSlider sizeMenuSlider = new("SIZE", 100, 100, 2000);

        public static string unitName { get; set; }

        public static bool pushComboStatus { get; set; }

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

        public static void ButtonDrawOn()
        {
            float scaling = RendererManager.Scaling;
            int optionsCount = _optionsCount;
            var positionX = positionSliderX;
            var positionY = positionSliderY;
            var sizeMenuX = sizeMenuSlider;
            float sizeMenuY = sizeMenuX / 1.2f;
            int indent = sizeMenuX / 5;

            var rectBase = new RectangleF(
                positionX,
                positionY,
                (int)(sizeMenuX + indent / 1.55f) * scaling * optionsCount,
                sizeMenuX / 4f * scaling);

            var rectText = new RectangleF(
                rectBase.X,
                rectBase.Y - 10 * scaling,
                rectBase.Width,
                rectBase.Height + 10 * scaling);

            var rect = new RectangleF(
                rectBase.X,
                rectBase.Y + rectBase.Height + 3 * scaling,
                rectBase.Width,
                (sizeMenuY + indent) * scaling * 3f);

            RendererManager.DrawFilledRectangle(rect, Color.Black, new Color(0, 0, 0, 250), 2);
            SizePanel = new Vector2((sizeMenuX + indent / 1.55f) * scaling * optionsCount, sizeMenuX / 4 * scaling);

            vector4PosClick.Clear();

            float fontSize = 35 * scaling;

            for (int i = 0; i < optionsCount; i++)
            {
                float posX = rect.X + indent / 2 * scaling + i * ((sizeMenuX + indent / 2) * scaling);
                float posY = rect.Y + indent / 2 * scaling;
                float width = sizeMenuX * scaling;
                float height = sizeMenuY * scaling;
                vector4PosClick.Add((Lane)i, new Vector4(posX, posY, width, height));

                var rectBorderImage = new RectangleF(
                    posX,
                    posY,
                    width,
                    height);

                var rectText1 = new RectangleF(
                    posX + 10 * scaling,
                    posY + 10 * scaling,
                    width,
                    height);

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

            var rectForCloneStatus = new RectangleF(rect.X + rect.Width * 0.5f - rect.Width / 3 * 0.5f, rect.Y + (sizeMenuX + indent * 0.5f) * scaling, rect.Width / 3f, sizeMenuY * scaling);

            if (unitName != null)
            {
                RendererManager.DrawRectangle(rectForCloneStatus, Color.Black);

                RendererManager.DrawImage(unitName, rectForCloneStatus, UnitImageType.Default, true);
            }

            if (pushComboStatus)
            {
                RendererManager.DrawRectangle(rectForCloneStatus, Color.Black);

                RendererManager.DrawText("Push combo ACTIVE", new Vector2(rect.X  , rect.Y + (sizeMenuX + indent * 0.5f) * scaling), Color.Red, fontSize);
            }

            var rectForItemsCd = new RectangleF(rect.X + rect.Width * 0.5f - rect.Width / 3 * 0.5f, rect.Y + (sizeMenuX + indent * 0.5f) * scaling * 2, rect.Width / 3f, sizeMenuY * scaling);
        }
    }
}