namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.FireRemnant
{
    using Base;

    using Core.Helpers;

    using Divine;

    using Helpers;

    internal class DrawableFireRemnantAbility : DrawableAbility
    {
        private readonly float addedTime;

        public DrawableFireRemnantAbility()
        {
            this.addedTime = GameManager.RawGameTime;
        }

        public override bool Draw
        {
            get
            {
                return this.Unit.IsValid && !this.Unit.IsVisible;
            }
        }

        public float Duration { get; set; }

        public override bool IsValid
        {
            get
            {
                if (GameManager.RawGameTime > this.ShowUntil)
                {
                    return false;
                }

                return this.Unit.IsValid && this.Unit.IsAlive;
            }
        }

        public Entity Owner { get; set; }

        public float ShowHeroUntil { get; set; }

        public Unit Unit { get; set; }

        public override void DrawOnMap(IMinimap minimap)
        {
            var position = minimap.WorldToScreen(this.Position, 35 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            var pct = (int)(((GameManager.RawGameTime - this.addedTime) / this.Duration) * 100);
            RendererManager.DrawTexture("o9k.outline_red", position * 1.2f);
            RendererManager.DrawTexture("o9k.outline_black" + pct, position * 1.25f);
            RendererManager.DrawTexture(this.AbilityTexture, position, TextureType.RoundAbility);
        }

        public override void DrawOnMinimap(IMinimap minimap)
        {
            if (GameManager.RawGameTime > this.ShowHeroUntil || this.Owner.IsVisible)
            {
                return;
            }

            var position = minimap.WorldToMinimap(this.Position, 25 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            RendererManager.DrawTexture("o9k.outline_red", position * 1.08f);
            RendererManager.DrawTexture(this.AbilityTexture, position, TextureType.RoundAbility);
        }
    }
}