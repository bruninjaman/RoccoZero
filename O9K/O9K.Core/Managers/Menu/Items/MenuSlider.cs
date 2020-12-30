namespace O9K.Core.Managers.Menu.Items
{
    using System;

    using Divine;

    using EventArgs;

    using Logger;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    public class MenuSlider : MenuItem
    {
        private readonly int defaultValue;

        private bool drag;

        private int value;

        private EventHandler<SliderEventArgs> valueChange;

        private Vector2 valueTextSize;

        public MenuSlider(string displayName, int value, int minValue, int maxValue, bool heroUnique = false)
            : this(displayName, displayName, value, minValue, maxValue, heroUnique)
        {
        }

        public MenuSlider(string displayName, string name, int value, int minValue, int maxValue, bool heroUnique = false)
            : base(displayName, name, heroUnique)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Value = value;
            this.defaultValue = value;
        }

        public event EventHandler<SliderEventArgs> ValueChange
        {
            add
            {
                value(this, new SliderEventArgs(this.value, this.value));
                this.valueChange += value;
            }
            remove
            {
                this.valueChange -= value;
            }
        }

        public int MaxValue { get; set; }

        public int MinValue { get; set; }

        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                var setValue = Math.Max(Math.Min(value, this.MaxValue), this.MinValue);
                if (setValue == this.value)
                {
                    return;
                }

                this.valueChange?.Invoke(this, new SliderEventArgs(setValue, this.value));
                this.value = setValue;

                if (this.SizeCalculated)
                {
                    this.valueTextSize = RendererManager.MeasureText(this.value.ToString(), this.MenuStyle.Font, this.MenuStyle.TextSize);
                }
            }
        }

        public static implicit operator int(MenuSlider item)
        {
            return item.value;
        }

        public static implicit operator float(MenuSlider item)
        {
            return item.value;
        }

        public MenuSlider SetTooltip(string tooltip)
        {
            this.LocalizedTooltip[Lang.En] = tooltip;
            return this;
        }

        internal override void CalculateSize()
        {
            base.CalculateSize();
            var maxTextSize = RendererManager.MeasureText(this.MaxValue.ToString(), this.MenuStyle.Font, this.MenuStyle.TextSize);
            this.Size = new Vector2(this.Size.X + maxTextSize.X, this.Size.Y);
        }

        internal override MenuItem GetItemUnder(Vector2 position)
        {
            if (this.drag)
            {
                return this;
            }

            return base.GetItemUnder(position);
        }

        internal override object GetSaveValue()
        {
            if (this.value == this.defaultValue)
            {
                return null;
            }

            return this.Value;
        }

        internal override void Load(JToken token)
        {
            try
            {
                token = token?[this.Name];
                if (token == null)
                {
                    return;
                }

                this.Value = (int)token;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal override bool OnMousePress(Vector2 position)
        {
            this.drag = true;
            this.SetValue(position);

            InputManager.MouseMove += this.OnMouseMove;
            InputManager.MouseKeyUp += this.OnMouseKeyUp;

            return true;
        }

        internal override bool OnMouseWheel(Vector2 position, bool up)
        {
            this.Value += up ? 1 : -1;
            return true;
        }

        internal override void Remove()
        {
            InputManager.MouseKeyUp -= this.OnMouseKeyUp;
            InputManager.MouseMove -= this.OnMouseMove;
        }

        internal override void SetRenderer()
        {
            base.SetRenderer();
            this.valueTextSize = RendererManager.MeasureText(this.value.ToString(), this.MenuStyle.Font, this.MenuStyle.TextSize);
        }

        protected override void Draw()
        {
            //value fill
            var fillPct = ((float)this.Value - this.MinValue) / (this.MaxValue - this.MinValue);

            RendererManager.DrawLine(
                this.Position + new Vector2(0, this.Size.Y / 2),
                this.Position + new Vector2(this.Size.X * fillPct, this.Size.Y / 2),
                this.MenuStyle.BackgroundColor,
                this.Size.Y);

            base.Draw();

            //value text
            var valuePosition = new Vector2(
                (this.Position.X + this.Size.X) - this.MenuStyle.RightIndent - this.valueTextSize.X,
                this.Position.Y + ((this.Size.Y - this.MenuStyle.TextSize) / 3.3f));

            RendererManager.DrawText(this.Value.ToString(), valuePosition, Color.White, this.MenuStyle.Font, this.MenuStyle.TextSize);
        }

        private void OnMouseKeyUp(MouseEventArgs e)
        {
            this.drag = false;
            InputManager.MouseKeyUp -= this.OnMouseKeyUp;
            InputManager.MouseMove -= this.OnMouseMove;
        }

        private void OnMouseMove(MouseMoveEventArgs e)
        {
            this.SetValue(e.Position);
        }

        private void SetValue(Vector2 position)
        {
            var start = this.Position.X;
            var end = start + this.Size.X;
            var pct = (position.X - start) / (end - start);

            this.Value = (int)((pct * (this.MaxValue - this.MinValue)) + this.MinValue);
        }
    }
}