namespace O9K.Core.Managers.Menu.Items
{
    using System;
    using System.Windows.Input;

    using Divine;

    using Logger;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using KeyEventArgs = EventArgs.KeyEventArgs;

    public class MenuToggleKey : MenuItem
    {
        private readonly bool defaultValue;

        private bool changingKey;

        private bool isActive;

        private string keyText;

        private Vector2 keyTextSize;

        private Key keyValue;

        private MouseKey mouseKeyValue;

        private EventHandler<KeyEventArgs> valueChange;

        public MenuToggleKey(string displayName, Key key = Key.None, bool defaultValue = true, bool heroUnique = false)
            : this(displayName, displayName, key, defaultValue, heroUnique)
        {
        }

        public MenuToggleKey(string displayName, string name, Key key = Key.None, bool defaultValue = true, bool heroUnique = false)
            : base(displayName, name, heroUnique)
        {
            this.IsActive = defaultValue;
            this.defaultValue = defaultValue;
            this.keyText = key.ToString();
            this.keyValue = key;
        }

        public event EventHandler<KeyEventArgs> ValueChange
        {
            add
            {
                if (this.isActive)
                {
                    value(this, new KeyEventArgs(this.isActive, this.isActive));
                }

                this.valueChange += value;
            }
            remove
            {
                this.valueChange -= value;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                if (this.isActive == value)
                {
                    return;
                }

                this.isActive = value;
                this.valueChange?.Invoke(this, new KeyEventArgs(this.isActive, !this.isActive));
            }
        }

        public Key Key
        {
            get
            {
                return this.keyValue;
            }
            set
            {
                this.keyValue = value;

                if (this.SizeCalculated)
                {
                    this.keyText = this.keyValue.ToString();
                    this.keyTextSize = RendererManager.MeasureText(this.keyText, this.MenuStyle.Font, this.MenuStyle.TextSize);
                }
            }
        }

        public MouseKey MouseKey
        {
            get
            {
                return this.mouseKeyValue;
            }
            set
            {
                this.mouseKeyValue = value;

                if (this.SizeCalculated)
                {
                    this.keyText = this.mouseKeyValue.ToString();
                    this.keyTextSize = RendererManager.MeasureText(this.keyText, this.MenuStyle.Font, this.MenuStyle.TextSize);
                }
            }
        }

        public static implicit operator bool(MenuToggleKey item)
        {
            return item.IsActive;
        }

        public MenuToggleKey SetTooltip(string tooltip)
        {
            this.LocalizedTooltip[Lang.En] = tooltip;
            return this;
        }

        internal override void CalculateSize()
        {
            base.CalculateSize();
            this.Size = new Vector2(this.Size.X + 40, this.Size.Y);
        }

        internal override object GetSaveValue()
        {
            if (this.MouseKey != MouseKey.None)
            {
                return new
                {
                    this.MouseKey,
                    this.IsActive
                };
            }

            return new
            {
                this.Key,
                IsActive = this.SaveValue ? this.IsActive : this.defaultValue
            };
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

                this.IsActive = token[nameof(this.IsActive)].ToObject<bool>();

                var key = token[nameof(this.Key)];
                if (key != null)
                {
                    this.Key = key.ToObject<Key>();
                    if (this.Key != Key.None)
                    {
                        InputManager.KeyUp += this.OnKeyUp;
                    }

                    return;
                }

                var mouseKey = token[nameof(this.MouseKey)];
                if (mouseKey != null)
                {
                    this.MouseKey = mouseKey.ToObject<MouseKey>();
                    if (this.MouseKey != MouseKey.None)
                    {
                        InputManager.MouseKeyUp += this.MouseKeyUp;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal override bool OnMouseRelease(Vector2 position)
        {
            if (this.changingKey)
            {
                return true;
            }

            this.Remove();
            this.changingKey = true;
            InputManager.KeyUp += this.GetKey;
            InputManager.MouseKeyUp += this.GetMouseKey;

            return true;
        }

        internal override void Remove()
        {
            InputManager.KeyUp -= this.OnKeyUp;
            InputManager.MouseKeyUp -= this.MouseKeyUp;
        }

        //internal override void SetInputManager(IInputManager9 inputManager)
        //{
        //    base.SetInputManager(inputManager);

        //    if (this.keyValue != Key.None)
        //    {
        //        this.InputManager.KeyUp += this.OnKeyUp;
        //    }
        //    else if (this.mouseKeyValue != MouseKey.None)
        //    {
        //        this.InputManager.MouseKeyUp += this.MouseKeyUp;
        //    }
        //}

        internal override void SetRenderer()
        {
            base.SetRenderer();
            this.keyTextSize = RendererManager.MeasureText(this.keyText, this.MenuStyle.Font, this.MenuStyle.TextSize);
        }

        protected override void Draw()
        {
            var keyPosition = new Vector2(
                (this.Position.X + this.Size.X) - this.MenuStyle.RightIndent - this.keyTextSize.X,
                this.Position.Y + ((this.Size.Y - this.MenuStyle.TextSize) / 3.3f));

            //key background
            if (this.IsActive)
            {
                RendererManager.DrawLine(
                    this.Position + new Vector2(this.Size.X - (this.keyTextSize.X + (this.MenuStyle.RightIndent * 2)), this.Size.Y / 2),
                    this.Position + new Vector2(this.Size.X, this.Size.Y / 2),
                    this.MenuStyle.BackgroundColor,
                    this.Size.Y);
            }

            base.Draw();

            //key
            RendererManager.DrawText(
                this.changingKey ? "?" : this.keyText,
                keyPosition,
                Color.White,
                this.MenuStyle.Font,
                this.MenuStyle.TextSize);
        }

        private void GetKey(Divine.KeyEventArgs e)
        {
            this.Key = e.Key == Key.Escape ? Key.None : e.Key;
            this.mouseKeyValue = MouseKey.None;
            e.Process = false;

            InputManager.KeyUp -= this.GetKey;
            InputManager.MouseKeyUp -= this.GetMouseKey;

            if (this.Key != Key.None)
            {
                InputManager.KeyUp += this.OnKeyUp;
            }

            this.changingKey = false;
        }

        private void GetMouseKey(MouseEventArgs e)
        {
            if (e.MouseKey == MouseKey.Left || e.MouseKey == MouseKey.Right)
            {
                this.keyValue = Key.None;
                this.MouseKey = MouseKey.None;
            }
            else
            {
                this.keyValue = Key.None;
                this.MouseKey = e.MouseKey;
            }

            e.Process = false;

            InputManager.KeyUp -= this.GetKey;
            InputManager.MouseKeyUp -= this.GetMouseKey;

            if (this.MouseKey != MouseKey.None)
            {
                InputManager.MouseKeyUp += this.MouseKeyUp;
            }

            this.changingKey = false;
        }

        private void MouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey == this.mouseKeyValue)
            {
                this.IsActive = !this.IsActive;
            }
        }

        private void OnKeyUp(Divine.KeyEventArgs e)
        {
            if (e.Key == this.keyValue)
            {
                this.IsActive = !this.IsActive;
            }
        }
    }
}