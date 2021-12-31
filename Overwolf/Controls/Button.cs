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
        private RectangleF buttonRect;
        public delegate void ButtonEventHandler(Button button, ButtonEventArgs e);
        public event ButtonEventHandler Click;
        private bool IsKeyDown;
        private bool Value;
        private readonly string TextureKey;
        private readonly float Scaling;

        public Button(string textureKey, float scaling = 1f)
        {
            buttonRect = new RectangleF();
            Scaling = scaling;
            TextureKey = textureKey;
            RendererManager.LoadImage(textureKey);
            InputManager.MouseKeyDown += InputManager_MouseKeyDown;
            InputManager.MouseKeyUp += InputManager_MouseKeyUp;
        }

        private void InputManager_MouseKeyDown(MouseEventArgs e)
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

        private void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            if (e.Position.IsUnderRectangle(buttonRect) && IsKeyDown)
            {
                Value = true;
                Click?.Invoke(this, new ButtonEventArgs(Value));
                Value = false;
            }
            IsKeyDown = false;
        }

        public void SetSize(RectangleF rect)
        {
            buttonRect = rect;
        }

        public void Draw()
        {
            //RendererManager.DrawFilledRectangle(buttonRect, new Color(0, 0, 0, 127));
            var scaleOffset = (buttonRect.Width - buttonRect.Width * Scaling) * 0.5f;
            RendererManager.DrawImage(TextureKey, new RectangleF(buttonRect.X + scaleOffset, buttonRect.Y + scaleOffset, buttonRect.Width * Scaling, buttonRect.Height * Scaling));
        }

    }
}
