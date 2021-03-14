namespace O9K.Core.Managers.Menu.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Divine;
    using Divine.SDK.Extensions;

    using EventArgs;

    using Logger;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    public class MenuAbilityPriorityChanger : MenuItem
    {
        private readonly Dictionary<string, (TextureType, bool)> abilities = new Dictionary<string, (TextureType, bool)>();

        private readonly Dictionary<string, int> abilityPriority = new Dictionary<string, int>();

        private readonly bool defaultValue;

        private readonly List<AbilityId> loadTextures = new List<AbilityId>();

        private readonly List<(string, TextureType)> loadTextures2 = new List<(string, TextureType)>();

        private readonly Dictionary<string, bool> savedAbilities = new Dictionary<string, bool>();

        private readonly Dictionary<string, int> savedAbilityPriority = new Dictionary<string, int>();

        private int autoPriority;

        private EventHandler<EventArgs> change;

        private Vector2 currentMousePosition;

        private bool drag;

        private string dragAbility;

        private Vector2 dragAbilityPosition;

        private string dragTargetAbility;

        private bool drawDrag;

        private bool increasePriority;

        private Vector2 mousePressDiff;

        private Vector2 mousePressPosition;

        public MenuAbilityPriorityChanger(
            string displayName,
            IDictionary<AbilityId, bool> abilities = null,
            bool defaultValue = true,
            bool heroUnique = false)
            : this(displayName, displayName, abilities, defaultValue, heroUnique)
        {
        }

        public MenuAbilityPriorityChanger(
            string displayName,
            string name,
            IDictionary<AbilityId, bool> abilities = null,
            bool defaultValue = true,
            bool heroUnique = false)
            : base(displayName, name, heroUnique)
        {
            this.defaultValue = defaultValue;

            if (abilities == null)
            {
                return;
            }

            foreach (var ability in abilities)
            {
                var abilityName = ability.Key.ToString();
                this.abilities[ability.Key.ToString()] = (TextureType.Ability, ability.Value);
                this.abilityPriority[abilityName] = this.autoPriority++;
                this.loadTextures.Add(ability.Key);
            }
        }

        public event EventHandler<EventArgs> Change
        {
            add
            {
                value(this, EventArgs.Empty);
                this.change += value;
            }
            remove
            {
                this.change -= value;
            }
        }

        public event EventHandler<AbilityPriorityEventArgs> OrderChange;

        public event EventHandler<AbilityEventArgs> ValueChange;

        public IEnumerable<string> Abilities
        {
            get
            {
                return this.abilities.Where(x => x.Value.Item2).OrderBy(x => this.abilityPriority[x.Key]).Select(x => x.Key);
            }
        }

        public void AddAbility(AbilityId id, bool? value = null, int priority = 0)
        {
            if (this.Renderer == null)
            {
                this.loadTextures.Add(id);
            }
            else
            {
                RendererManager.LoadTexture(id);
            }

            this.AddAbility(id.ToString(), value, priority);
        }

        public void AddAbility(string name, bool? value = null, int priority = 0)
        {
            if (this.abilities.ContainsKey(name))
            {
                return;
            }

            var textureType = TextureType.Default;

            if (name.Contains("npc_dota"))
            {
                textureType = TextureType.Unit;
            }
            else if (Enum.IsDefined(typeof(AbilityId), name))
            {
                textureType = TextureType.Ability;
            }

            if (this.Renderer == null)
            {
                this.loadTextures2.Add((name, textureType));
            }
            else
            {
                RendererManager.LoadTexture(name, textureType);
            }

            if (this.savedAbilities.TryGetValue(name, out var savedValue))
            {
                this.abilities[name] = (textureType, savedValue);
            }
            else
            {
                this.abilities[name] = (textureType, value ?? this.defaultValue);
            }

            if (this.savedAbilityPriority.TryGetValue(name, out var savedPriority))
            {
                this.abilityPriority[name] = savedPriority;
            }
            else
            {
                if (priority != 0)
                {
                    this.abilityPriority[name] = this.TryGetPriority(priority);
                }
                else
                {
                    this.abilityPriority[name] = this.autoPriority++;
                }
            }

            if (this.SizeCalculated)
            {
                this.CalculateSize();
            }
        }

        public int GetPriority(string name)
        {
            if (this.abilityPriority.TryGetValue(name, out var value))
            {
                return value;
            }

            return 99999;
        }

        public bool IsEnabled(string name)
        {
            this.abilities.TryGetValue(name, out var value);
            return value.Item2;
        }

        public MenuAbilityPriorityChanger SetTooltip(string tooltip)
        {
            this.LocalizedTooltip[Lang.En] = tooltip;
            return this;
        }

        internal override void CalculateSize()
        {
            this.DisplayNameSize = RendererManager.MeasureText(this.DisplayName, this.MenuStyle.Font, this.MenuStyle.TextSize);
            var width = this.DisplayNameSize.X + this.MenuStyle.LeftIndent + this.MenuStyle.RightIndent + 10
                        + (this.MenuStyle.TextureArrowSize * 2) + (this.abilities.Count * this.MenuStyle.TextureAbilitySize);
            this.Size = new Vector2(width, this.MenuStyle.Height);
            this.ParentMenu.CalculateWidth();

            this.SizeCalculated = true;
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
            foreach (var ability in this.abilities)
            {
                this.savedAbilities[ability.Key] = ability.Value.Item2;
            }

            foreach (var ability in this.abilityPriority)
            {
                this.savedAbilityPriority[ability.Key] = ability.Value;
            }

            return new
            {
                Abilities = this.savedAbilities,
                Priority = this.savedAbilityPriority
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

                foreach (var item in token["Abilities"].ToObject<JObject>())
                {
                    var key = item.Key;
                    var value = (bool)item.Value;

                    this.savedAbilities[key] = value;

                    if (this.abilities.TryGetValue(key, out var v))
                    {
                        this.abilities[key] = (v.Item1, value);
                    }
                }

                foreach (var item in token["Priority"].ToObject<JObject>())
                {
                    var key = item.Key;
                    var value = (int)item.Value;

                    this.savedAbilityPriority[key] = value;

                    if (value >= this.autoPriority)
                    {
                        this.autoPriority = value + 1;
                    }

                    if (this.abilityPriority.ContainsKey(key))
                    {
                        this.abilityPriority[key] = value;
                    }
                }

                this.change?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal override bool OnMousePress(Vector2 position)
        {
            if (this.abilities.Count == 0)
            {
                return false;
            }

            var startPosition = new Vector2(
                (this.Position.X + this.Size.X) - this.MenuStyle.TextureAbilitySize - 4,
                this.Position.Y + ((this.Size.Y - this.MenuStyle.TextureAbilitySize) / 2.2f));

            foreach (var ability in this.abilities.OrderByDescending(x => this.abilityPriority[x.Key]))
            {
                var abilityPosition = new RectangleF(
                    startPosition.X - 1.5f,
                    startPosition.Y - 1.5f,
                    this.MenuStyle.TextureAbilitySize + 3,
                    this.MenuStyle.TextureAbilitySize + 3);

                if (abilityPosition.Contains(position))
                {
                    this.currentMousePosition = position;
                    this.mousePressPosition = position;
                    this.mousePressDiff = position - startPosition;
                    this.dragAbilityPosition = position - this.mousePressDiff;
                    this.dragAbility = ability.Key;
                    this.drag = true;

                    InputManager.MouseMove += this.OnMouseMove;
                    InputManager.MouseKeyUp += this.OnMouseKeyUp;

                    return true;
                }

                startPosition -= new Vector2(this.MenuStyle.TextureAbilitySize + 4, 0);
            }

            return false;
        }

        internal override bool OnMouseRelease(Vector2 position)
        {
            if (this.abilities.Count == 0)
            {
                return false;
            }

            if (this.drawDrag)
            {
                this.drag = false;
                this.drawDrag = false;

                if (string.IsNullOrEmpty(this.dragTargetAbility) || this.dragTargetAbility == this.dragAbility)
                {
                    return false;
                }

                var currentPriority = this.abilityPriority[this.dragAbility];
                int setPriority;

                if (this.increasePriority)
                {
                    setPriority = this.abilityPriority[this.dragTargetAbility] - 1;

                    foreach (var key in this.abilityPriority.Where(x => x.Value <= setPriority).Select(x => x.Key).ToList())
                    {
                        this.abilityPriority[key]--;
                    }

                    this.abilityPriority[this.dragAbility] = setPriority;
                    this.increasePriority = false;
                }
                else
                {
                    setPriority = this.abilityPriority[this.dragTargetAbility] + 1;

                    foreach (var key in this.abilityPriority.Where(x => x.Value >= setPriority).Select(x => x.Key).ToList())
                    {
                        this.abilityPriority[key]++;
                    }

                    this.abilityPriority[this.dragAbility] = setPriority;
                }

                this.autoPriority = this.abilityPriority.Values.Max() + 1;
                this.OrderChange?.Invoke(this, new AbilityPriorityEventArgs(this.dragAbility, setPriority, currentPriority));
                this.change?.Invoke(this, EventArgs.Empty);

                return true;
            }

            var startPosition = new Vector2(
                (this.Position.X + this.Size.X) - this.MenuStyle.TextureAbilitySize - 4,
                this.Position.Y + ((this.Size.Y - this.MenuStyle.TextureAbilitySize) / 2.2f));

            foreach (var ability in this.abilities.OrderByDescending(x => this.abilityPriority[x.Key]))
            {
                var abilityPosition = new RectangleF(
                    startPosition.X - 1.5f,
                    startPosition.Y - 1.5f,
                    this.MenuStyle.TextureAbilitySize + 3,
                    this.MenuStyle.TextureAbilitySize + 3);

                if (abilityPosition.Contains(position))
                {
                    var value = this.abilities[ability.Key];
                    this.abilities[ability.Key] = (value.Item1, !value.Item2);
                    this.ValueChange?.Invoke(this, new AbilityEventArgs(ability.Key, !value.Item2, value.Item2));
                    this.change?.Invoke(this, EventArgs.Empty);

                    return true;
                }

                startPosition -= new Vector2(this.MenuStyle.TextureAbilitySize + 4, 0);
            }

            return false;
        }

        internal override void Remove()
        {
            InputManager.MouseKeyUp -= this.OnMouseKeyUp;
            InputManager.MouseMove -= this.OnMouseMove;
        }

        internal override void SetRenderer()
        {
            base.SetRenderer();

            foreach (var texture in this.loadTextures)
            {
                RendererManager.LoadTexture(texture);
            }

            foreach (var texture in this.loadTextures2)
            {
                RendererManager.LoadTexture(texture.Item1, texture.Item2);
            }
        }

        protected override void Draw()
        {
            base.Draw();

            //drag ability
            if (this.drawDrag)
            {
                RendererManager.DrawTexture(
                    this.dragAbility,
                    new RectangleF(
                        this.dragAbilityPosition.X,
                        this.dragAbilityPosition.Y,
                        this.MenuStyle.TextureAbilitySize,
                        this.MenuStyle.TextureAbilitySize));
                this.dragTargetAbility = null;
            }

            //abilities
            var startPosition = new Vector2(
                (this.Position.X + this.Size.X) - this.MenuStyle.TextureAbilitySize - 4,
                this.Position.Y + ((this.Size.Y - this.MenuStyle.TextureAbilitySize) / 2.2f));

            var priority = this.abilities.Count(x => x.Value.Item2);
            var count = 0;
            foreach (var ability in this.abilities.OrderByDescending(x => this.abilityPriority[x.Key]))
            {
                count++;

                var isEnabled = ability.Value.Item2;

                if (this.drawDrag)
                {
                    var border = 3;
                    if (ability.Key == this.dragAbility)
                    {
                        border = 0;
                    }

                    if ((count == 1 && this.currentMousePosition.X > this.Position.X + this.Size.X)
                        || (this.currentMousePosition.X >= startPosition.X - border && this.currentMousePosition.X
                            <= startPosition.X + this.MenuStyle.TextureAbilitySize + border))
                    {
                        this.dragTargetAbility = ability.Key;
                        startPosition -= new Vector2(this.MenuStyle.TextureAbilitySize + 4, 0);
                        this.increasePriority = false;
                    }

                    if (ability.Key == this.dragAbility)
                    {
                        if (isEnabled)
                        {
                            priority--;
                        }

                        continue;
                    }
                }

                RendererManager.DrawRectangle(
                    new RectangleF(
                        startPosition.X - 1.5f,
                        startPosition.Y - 1.5f,
                        this.MenuStyle.TextureAbilitySize + 3,
                        this.MenuStyle.TextureAbilitySize + 3),
                    isEnabled ? Color.LightGreen : Color.Red,
                    1.5f);
                RendererManager.DrawTexture(
                    ability.Key,
                    new RectangleF(startPosition.X, startPosition.Y, this.MenuStyle.TextureAbilitySize, this.MenuStyle.TextureAbilitySize),
                    ability.Value.Item1);

                if (isEnabled)
                {
                    //priority
                    RendererManager.DrawLine(
                        startPosition + new Vector2(0, this.MenuStyle.TextureAbilitySize - 6),
                        startPosition + new Vector2(6, this.MenuStyle.TextureAbilitySize - 6),
                        Color.Black,
                        12);
                    RendererManager.DrawText(
                        priority--.ToString(),
                        startPosition + new Vector2(0, this.MenuStyle.TextureAbilitySize - 12),
                        Color.White,
                        this.MenuStyle.Font,
                        12);
                }

                startPosition -= new Vector2(this.MenuStyle.TextureAbilitySize + 4, 0);
            }

            if (this.drawDrag && this.dragTargetAbility == null)
            {
                this.dragTargetAbility = this.abilities.Select(x => x.Key).OrderBy(x => this.abilityPriority[x]).FirstOrDefault();
                this.increasePriority = true;
            }
        }

        private void OnMouseKeyUp(MouseEventArgs e)
        {
            this.drag = false;
            this.drawDrag = false;
            InputManager.MouseKeyUp -= this.OnMouseKeyUp;
            InputManager.MouseMove -= this.OnMouseMove;
        }

        private void OnMouseMove(MouseMoveEventArgs e)
        {
            this.currentMousePosition = e.Position;
            this.dragAbilityPosition = e.Position - this.mousePressDiff;
            this.drawDrag = this.mousePressPosition.Distance(e.Position) > 5;
        }

        private int TryGetPriority(int priority)
        {
            while (this.abilityPriority.Values.Any(x => x == priority) || this.savedAbilityPriority.Values.Any(x => x == priority))
            {
                priority++;
            }

            if (priority >= this.autoPriority)
            {
                this.autoPriority = priority + 1;
            }

            return priority;
        }
    }
}