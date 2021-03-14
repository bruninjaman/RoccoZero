namespace O9K.Core.Managers.Menu.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Divine;

    using EventArgs;

    using Logger;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    public class MenuAbilityToggler : MenuItem
    {
        private readonly Dictionary<string, (TextureType, bool)> abilities = new Dictionary<string, (TextureType, bool)>();

        private readonly bool defaultValue;

        private readonly List<AbilityId> loadTextures = new List<AbilityId>();

        private readonly List<(string, TextureType)> loadTextures2 = new List<(string, TextureType)>();

        private readonly Dictionary<string, bool> savedAbilities = new Dictionary<string, bool>();

        private bool loaded;

        private EventHandler<AbilityEventArgs> valueChange;

        public MenuAbilityToggler(
            string displayName,
            IDictionary<AbilityId, bool> abilities = null,
            bool defaultValue = true,
            bool heroUnique = false)
            : this(displayName, displayName, abilities, defaultValue, heroUnique)
        {
        }

        public MenuAbilityToggler(
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
                this.abilities[ability.Key.ToString()] = (TextureType.Ability, ability.Value);
                this.loadTextures.Add(ability.Key);
            }
        }

        public event EventHandler<AbilityEventArgs> ValueChange
        {
            add
            {
                if (this.loaded)
                {
                    foreach (var ability in this.Abilities)
                    {
                        value(this, new AbilityEventArgs(ability, true, true));
                    }
                }

                this.valueChange += value;
            }
            remove
            {
                this.valueChange -= value;
            }
        }

        public IEnumerable<string> Abilities
        {
            get
            {
                return this.abilities.Where(x => x.Value.Item2).Select(x => x.Key);
            }
        }

        public IReadOnlyDictionary<string, bool> AllAbilities
        {
            get
            {
                return this.abilities.ToDictionary(x => x.Key, x => x.Value.Item2);
            }
        }

        public void AddAbility(AbilityId id, bool? value = null)
        {
            if (this.Renderer == null)
            {
                this.loadTextures.Add(id);
            }
            else
            {
                RendererManager.LoadTexture(id);
            }

            this.AddAbility(id.ToString(), value);
        }

        public void AddAbility(string name, bool? value = null)
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

            if (this.abilities[name].Item2)
            {
                this.valueChange?.Invoke(this, new AbilityEventArgs(name, true, true));
            }

            if (this.SizeCalculated)
            {
                this.CalculateSize();
            }
        }

        public bool IsEnabled(string name)
        {
            this.abilities.TryGetValue(name, out var value);
            return value.Item2;
        }

        public MenuAbilityToggler SetTooltip(string tooltip)
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

        internal override object GetSaveValue()
        {
            foreach (var ability in this.abilities)
            {
                this.savedAbilities[ability.Key] = ability.Value.Item2;
            }

            return this.savedAbilities;
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

                foreach (var item in token.ToObject<JObject>())
                {
                    var key = item.Key;
                    var value = (bool)item.Value;

                    this.savedAbilities[key] = value;

                    if (this.abilities.TryGetValue(key, out var v))
                    {
                        this.abilities[key] = (v.Item1, value);
                    }
                }

                foreach (var ability in this.Abilities)
                {
                    this.valueChange?.Invoke(this, new AbilityEventArgs(ability, true, true));
                }

                this.loaded = true;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal override bool OnMouseRelease(Vector2 position)
        {
            if (this.abilities.Count == 0)
            {
                return false;
            }

            var startPosition = new Vector2(
                (this.Position.X + this.Size.X) - this.MenuStyle.TextureAbilitySize - 4,
                this.Position.Y + ((this.Size.Y - this.MenuStyle.TextureAbilitySize) / 2.2f));

            foreach (var ability in this.abilities.Reverse())
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
                    this.valueChange?.Invoke(this, new AbilityEventArgs(ability.Key, !value.Item2, value.Item2));
                    return true;
                }

                startPosition -= new Vector2(this.MenuStyle.TextureAbilitySize + 4, 0);
            }

            return false;
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

            //abilities
            var startPosition = new Vector2(
                (this.Position.X + this.Size.X) - this.MenuStyle.TextureAbilitySize - 4,
                this.Position.Y + ((this.Size.Y - this.MenuStyle.TextureAbilitySize) / 2.2f));

            foreach (var ability in this.abilities.Reverse())
            {
                RendererManager.DrawRectangle(
                    new RectangleF(
                        startPosition.X - 1.5f,
                        startPosition.Y - 1.5f,
                        this.MenuStyle.TextureAbilitySize + 3,
                        this.MenuStyle.TextureAbilitySize + 3),
                    ability.Value.Item2 ? Color.LightGreen : Color.Red,
                    1.5f);
                RendererManager.DrawTexture(
                    ability.Key,
                    new RectangleF(startPosition.X, startPosition.Y, this.MenuStyle.TextureAbilitySize, this.MenuStyle.TextureAbilitySize),
                    ability.Value.Item1);

                startPosition -= new Vector2(this.MenuStyle.TextureAbilitySize + 4, 0);
            }
        }
    }
}