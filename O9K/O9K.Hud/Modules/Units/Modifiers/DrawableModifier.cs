namespace O9K.Hud.Modules.Units.Modifiers
{
    using Divine;

    internal class DrawableModifier
    {
        private float endTime;

        public DrawableModifier(Modifier modifier, Modifiers.ModifierType type)
        {
            this.Modifier = modifier;
            this.Index = modifier.Index;
            this.IsDebuff = modifier.IsDebuff;
            this.IgnoreTime = type != Modifiers.ModifierType.Temporary;
            this.IsHiddenAura = type == Modifiers.ModifierType.Aura && modifier.IsHidden;
            this.CreateTime = GameManager.RawGameTime;

            if (this.IsHiddenAura && modifier.Name == "modifier_truesight")
            {
                //no texture name
                this.TextureName = "o9k.modifier_truesight";
            }
            else
            {
                this.TextureName = modifier.TextureName;
                this.IsAbilityTextureName = true;
            }

            this.UpdateTimings();
        }

        public float CreateTime { get; }

        public float Duration { get; private set; }

        public int Index { get; }

        public bool IgnoreTime { get; }

        public bool IsDebuff { get; }

        public bool IsHiddenAura { get; }

        public Modifier Modifier { get; }

        public bool IsAbilityTextureName { get; }

        public float RemainingTime
        {
            get
            {
                return this.endTime - GameManager.RawGameTime;
            }
        }

        public bool ShouldDraw
        {
            get
            {
                return this.IgnoreTime || this.RemainingTime > 0;
            }
        }

        public string TextureName { get; }

        public void UpdateTimings()
        {
            if (this.IgnoreTime)
            {
                return;
            }

            this.Duration = this.Modifier.Duration;
            this.endTime = (GameManager.RawGameTime - this.Modifier.ElapsedTime) + this.Duration;
        }
    }
}