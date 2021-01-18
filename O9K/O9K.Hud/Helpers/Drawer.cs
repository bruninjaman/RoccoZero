namespace O9K.Hud.Helpers
{
    using Divine;

    using SharpDX;

    internal static class Drawer
    {
        public static void DrawTextWithBackground(string text, float size, Vector2 position)
        {
            var measureText = RendererManager.MeasureText(text, size);
            position -= new Vector2(measureText.X / 2, 0);
            var bgPosition = position + new Vector2(0, measureText.Y / 2);

            RendererManager.DrawLine(
                bgPosition - new Vector2(2, 0),
                bgPosition + new Vector2(measureText.X + 2, 0),
                new Color(5, 25, 25, 150),
                measureText.Y);
            RendererManager.DrawText(text, position, Color.White, size);
        }
    }
}