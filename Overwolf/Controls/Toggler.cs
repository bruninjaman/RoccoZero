using Divine.Extensions;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;

using Overwolf.Controls.EventArgs;

namespace Overwolf.Renderer
{
    internal sealed class Toggler
    {
        private RectangleF togglerRect;
        public delegate void TooglerEventHandler(Toggler button, TooglerEventArgs e);
        public event TooglerEventHandler ValueChanged;
        public bool Value;
        private bool IsKeyDown;
        private readonly string TextureKey;
        private readonly float Scaling;

        public Toggler(string textureKey, float scaling = 1f)
        {
            togglerRect = new RectangleF();
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

            if (e.Position.IsUnderRectangle(togglerRect))
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

            if (e.Position.IsUnderRectangle(togglerRect) && IsKeyDown)
            {
                Value = !Value;
                ValueChanged?.Invoke(this, new TooglerEventArgs());
            }
            IsKeyDown = false;
        }

        public void SetRectangle(RectangleF rect)
        {
            togglerRect = rect;
        }

        public void Draw()
        {
            //RendererManager.DrawFilledRectangle(togglerRect, new Color(0, 0, 0, 127));
            var scaleOffset = (togglerRect.Width - (togglerRect.Width * Scaling)) * 0.5f;
            RendererManager.DrawImage(TextureKey, new RectangleF(togglerRect.X + scaleOffset, togglerRect.Y + scaleOffset, togglerRect.Width * Scaling, togglerRect.Height * Scaling));
        }
    }
}
