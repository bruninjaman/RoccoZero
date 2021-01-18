namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Base
{
    using Core.Helpers;

    using Divine;

    using Helpers;

    using SharpDX;

    internal class DrawableAbility : IDrawableAbility
    {
        protected Particle RangeParticle;

        public AbilityId AbilityId { get; set; }

        public string AbilityTexture { get; set; }

        public virtual bool Draw { get; } = true;

        public string HeroTexture { get; set; }

        public bool IsShowingRange { get; set; }

        public virtual bool IsValid
        {
            get
            {
                return GameManager.RawGameTime < this.ShowUntil;
            }
        }

        public string MinimapHeroTexture { get; set; }

        public Vector3 Position { get; set; }

        public float Range { get; set; }

        public Vector3 RangeColor { get; set; }

        public float ShowUntil { get; set; }

        public virtual void DrawOnMap(IMinimap minimap)
        {
            var position = minimap.WorldToScreen(this.Position, 45 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            RendererManager.DrawTexture("o9k.outline_red", position * 1.12f);
            RendererManager.DrawTexture(this.HeroTexture, position, UnitTextureType.RoundUnit);

            var abilityTexturePosition = position * 0.5f;
            abilityTexturePosition.X += abilityTexturePosition.Width * 0.8f;
            abilityTexturePosition.Y += abilityTexturePosition.Height * 0.6f;

            RendererManager.DrawTexture("o9k.outline_green_pct100", abilityTexturePosition * 1.2f);
            RendererManager.DrawTexture(this.AbilityTexture, abilityTexturePosition, TextureType.RoundAbility);
        }

        public virtual void DrawOnMinimap(IMinimap minimap)
        {
            var position = minimap.WorldToMinimap(this.Position, 25 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            RendererManager.DrawTexture("o9k.outline_red", position * 1.08f);
            RendererManager.DrawTexture(this.MinimapHeroTexture, position, UnitTextureType.MiniUnit);
        }

        public void DrawRange()
        {
            if (!this.IsShowingRange)
            {
                return;
            }

            if (this.RangeParticle != null)
            {
                this.RangeParticle.SetControlPoint(0, this.Position);
                return;
            }

            this.RangeParticle = ParticleManager.CreateParticle("particles/ui_mouseactions/drag_selected_ring.vpcf", this.Position);
            this.RangeParticle.SetControlPoint(1, this.RangeColor);
            this.RangeParticle.SetControlPoint(2, new Vector3(-this.Range, 255, 0));
        }

        public virtual void RemoveRange()
        {
            if (this.RangeParticle?.IsValid == true)
            {
                this.RangeParticle.Dispose();
            }
        }
    }
}