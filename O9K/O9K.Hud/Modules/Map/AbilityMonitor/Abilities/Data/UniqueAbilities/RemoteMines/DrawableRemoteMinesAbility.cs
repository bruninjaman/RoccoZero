namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.RemoteMines
{
    using Base;

    using Core.Helpers;
    using Core.Managers.Renderer.Utils;

    using Divine;

    using Helpers;

    using SharpDX;

    internal class DrawableRemoteMinesAbility : DrawableAbility
    {
        private readonly float addedTime;

        public DrawableRemoteMinesAbility()
        {
            this.addedTime = GameManager.RawGameTime;
        }

        public Entity BaseEntity { get; set; }

        public override bool Draw
        {
            get
            {
                return this.Unit == null || (this.Unit.IsValid && !this.Unit.IsVisible);
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

                return this.Unit == null || (this.Unit.IsValid && this.Unit.IsAlive);
            }
        }

        public float ShowHeroUntil { get; set; }

        public Unit Unit { get; set; }

        public void AddUnit(Unit unit)
        {
            this.Unit = unit;
        }

        public override void DrawOnMap(IMinimap minimap)
        {
            var position = RendererManager.WorldToScreen(this.Position);
            if (position.IsZero)
            {
                return;
            }

            var time = GameManager.RawGameTime;

            if (time < this.ShowHeroUntil)
            {
                var heroTexturePosition = new Rectangle9(position, new Vector2(45));
                RendererManager.DrawTexture("o9k.outline_red", heroTexturePosition * 1.12f);
                RendererManager.DrawTexture(this.HeroTexture, heroTexturePosition);

                var abilityTexturePosition = new Rectangle9(position + new Vector2(30, 20), new Vector2(27));
                RendererManager.DrawTexture("o9k.outline_green_pct100", abilityTexturePosition * 1.2f);
                RendererManager.DrawTexture(this.AbilityTexture, abilityTexturePosition, TextureType.RoundAbility);
            }
            else
            {
                var pct = (int)(((time - this.addedTime) / this.Duration) * 100);
                var abilityTexturePosition = new Rectangle9(position - (new Vector2(35) / 2f), new Vector2(35));
                RendererManager.DrawTexture("o9k.outline_red", abilityTexturePosition * 1.2f);
                RendererManager.DrawTexture("o9k.outline_black" + pct, abilityTexturePosition * 1.25f);
                RendererManager.DrawTexture(this.AbilityTexture, abilityTexturePosition, TextureType.RoundAbility);
            }
        }

        public override void DrawOnMinimap(IMinimap minimap)
        {
            if (GameManager.RawGameTime > this.ShowHeroUntil || this.BaseEntity.IsVisible)
            {
                return;
            }

            var position = minimap.WorldToMinimap(this.Position, 25 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            RendererManager.DrawTexture("o9k.outline_red", position * 1.08f);
            RendererManager.DrawTexture(this.MinimapHeroTexture, position, UnitTextureType.MiniUnit);
        }
    }
}