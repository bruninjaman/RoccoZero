using Divine.Numerics;
using Divine.Renderer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowEnemyRoles.Exstensions
{
    internal static class DrawTextExstensions
    {
        internal static void DrawTextWithShadow(string text, Divine.Numerics.RectangleF rect, Divine.Numerics.Color color, string fontFamilyName, float fontSize, bool fontShadow)
        {
            if (fontShadow)
                RendererManager.DrawText(
                    text,
                    new RectangleF(rect.X + 1f, rect.Y + 1f, rect.Width, rect.Height),
                    Color.Black,
                    fontFamilyName,
                    fontSize);
            RendererManager.DrawText(
                text,
                rect,
                color,
                fontFamilyName,
                fontSize);
        }

        internal static void DrawTextWithShadow(string text, Divine.Numerics.RectangleF rect, Divine.Numerics.Color color, string fontFamilyName, FontFlags fontFlags, float fontSize, bool fontShadow)
        {
            if (fontShadow)
                RendererManager.DrawText(
                    text,
                    new RectangleF(rect.X + 1f, rect.Y + 1f, rect.Width, rect.Height),
                    Color.Black,
                    fontFamilyName,
                    fontFlags,
                    fontSize);
            RendererManager.DrawText(
                text,
                rect,
                color,
                fontFamilyName,
                fontFlags,
                fontSize);
        }

        internal static void DrawTextWithShadow(string text, Divine.Numerics.RectangleF rect, Divine.Numerics.Color color, string fontFamilyName, FontWeight fontWeight, FontFlags fontFlags, float fontSize, bool fontShadow)
        {
            if (fontShadow)
                RendererManager.DrawText(
                    text,
                    new RectangleF(rect.X + 1f, rect.Y + 1f, rect.Width, rect.Height),
                    Color.Black,
                    fontFamilyName,
                    fontWeight,
                    fontFlags,
                    fontSize);
            RendererManager.DrawText(
                text,
                rect,
                color,
                fontFamilyName,
                fontWeight,
                fontFlags,
                fontSize);
        }
    }
}
