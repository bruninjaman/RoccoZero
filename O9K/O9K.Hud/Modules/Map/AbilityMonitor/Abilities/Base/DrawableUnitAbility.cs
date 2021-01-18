namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Base
{
    using System;

    using Core.Helpers;

    using Divine;

    using Helpers;

    using SharpDX;

    internal class DrawableUnitAbility : IDrawableAbility
    {
        protected Particle RangeParticle;

        private readonly float addedTime;

        public DrawableUnitAbility()
        {
            this.addedTime = GameManager.RawGameTime;
        }

        public AbilityId AbilityId { get; set; }

        public string AbilityTexture { get; set; }

        public bool Draw
        {
            get
            {
                return !this.Unit.IsVisible;
            }
        }

        public float Duration { get; set; }

        public string HeroTexture { get; set; }

        public bool IsShowingRange { get; set; }

        public bool IsValid
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

        public string MinimapHeroTexture { get; set; }

        public Entity Owner { get; set; }

        public Vector3 Position { get; set; }

        public float Range { get; set; }

        public Vector3 RangeColor { get; set; }

        public float ShowHeroUntil { get; set; }

        public bool ShowTimer { get; set; }

        public float ShowUntil { get; set; }

        public Unit Unit { get; set; }

        public void DrawOnMap(IMinimap minimap)
        {
            var position = minimap.WorldToScreen(this.Position, 45 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            var time = GameManager.RawGameTime;

            if (time < this.ShowHeroUntil)
            {
                RendererManager.DrawTexture("o9k.outline_red", position * 1.12f);
                RendererManager.DrawTexture(this.HeroTexture, position, UnitTextureType.RoundUnit);

                var abilityTexturePosition = position * 0.5f;
                abilityTexturePosition.X += abilityTexturePosition.Width * 0.8f;
                abilityTexturePosition.Y += abilityTexturePosition.Height * 0.6f;

                RendererManager.DrawTexture("o9k.outline_green_pct100", abilityTexturePosition * 1.2f);
                RendererManager.DrawTexture(this.AbilityTexture, abilityTexturePosition, TextureType.RoundAbility);
            }
            else
            {
                RendererManager.DrawTexture("o9k.outline_red", position);

                if (this.ShowTimer)
                {
                    var pct = (int)(((time - this.addedTime) / this.Duration) * 100);
                    RendererManager.DrawTexture("o9k.outline_black" + Math.Min(pct, 100), position * 1.05f);
                }

                RendererManager.DrawTexture(this.AbilityTexture, position * 0.8f, TextureType.RoundAbility);
            }
        }

        public void DrawOnMinimap(IMinimap minimap)
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