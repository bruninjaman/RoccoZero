using Divine.Extensions;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;

using Overwolf.Controls.EventArgs;

using System;

namespace Overwolf.Renderer
{
    internal sealed class Toggler
    {
        internal delegate void TooglerEventHandler(Toggler button, TooglerEventArgs e);
        internal event TooglerEventHandler ValueChanged;
        private RectangleF togglerRect;
        private bool IsKeyDown;
        private bool mouseHovered;
        private readonly string TextureKey;
        internal bool Value { get; private set; }

        internal Toggler(string textureKey)
        {
            togglerRect = new RectangleF();
            TextureKey = textureKey;
            RendererManager.LoadImage(textureKey);;
        }

        internal void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            if (e.Position.IsUnderRectangle(togglerRect))
            {
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

            if (e.Position.IsUnderRectangle(togglerRect))
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

            if (e.Position.IsUnderRectangle(togglerRect) && IsKeyDown)
            {
                Value = !Value;
                ValueChanged?.Invoke(this, new TooglerEventArgs(Value));
            }
            IsKeyDown = false;
        }

        internal void SetRectangle(RectangleF rect)
        {
            togglerRect = rect;
        }

        internal void Draw()
        {
            if (mouseHovered)
            {
                RendererManager.DrawImage("Overwolf.SoftRectangle", togglerRect);
            }
            //RendererManager.DrawFilledRectangle(togglerRect, new Color(0, 0, 0, 127));
            RendererManager.DrawImage(TextureKey, new RectangleF(togglerRect.X + (togglerRect.Width * 0.1f), togglerRect.Y + (togglerRect.Width * 0.1f), togglerRect.Width * 0.8f, togglerRect.Height * 0.8f));
        }
    }
}