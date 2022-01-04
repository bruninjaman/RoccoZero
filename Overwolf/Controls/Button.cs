using Divine.Extensions;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;

using Overwolf.Controls.EventArgs;

namespace Overwolf.Controls
{
    internal sealed class Button
    {
        internal delegate void ButtonEventHandler(Button button, ButtonEventArgs e);
        internal event ButtonEventHandler Click;
        private RectangleF buttonRect;
        private bool IsKeyDown;
        private bool mouseHovered;
        private readonly string TextureKey;

        internal Button(string textureKey)
        {
            buttonRect = new RectangleF();
            TextureKey = textureKey;
            RendererManager.LoadImage(textureKey);
        }

        internal void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            if (e.Position.IsUnderRectangle(buttonRect))
            {
                //System.Console.WriteLine("KEK");
                mouseHovered = true;
            }
            else
            {
                mouseHovered = false;
            }
        }

        internal void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (e.Position.IsUnderRectangle(buttonRect))
            {
                IsKeyDown = true;
            }
        }

        internal void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (e.Position.IsUnderRectangle(buttonRect) && IsKeyDown)
            {
                Click?.Invoke(this, new ButtonEventArgs(IsKeyDown));
            }
            IsKeyDown = false;
        }

        internal void SetRectangle(RectangleF rect)
        {
            buttonRect = rect;
        }

        internal void Draw()
        {
            if (mouseHovered)
            {
                RendererManager.DrawImage("Overwolf.SoftRectangle", buttonRect);
            }
            //RendererManager.DrawFilledRectangle(buttonRect, new Color(0, 0, 0, 127));

            RendererManager.DrawImage(TextureKey, new RectangleF(buttonRect.X, buttonRect.Y, buttonRect.Width, buttonRect.Height));
        }
    }
}