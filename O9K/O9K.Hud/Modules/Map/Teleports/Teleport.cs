namespace O9K.Hud.Modules.Map.Teleports
{
    using System;

    using Core.Helpers;

    using Divine;

    using Helpers;

    using SharpDX;

    internal sealed class Teleport
    {
        private const string AbilityTexture = nameof(AbilityId.item_tpscroll);

        private readonly Color color;

        private readonly float displayUntil;

        private readonly float duration;

        private readonly string mapTexture;

        private readonly string minimapTexture;

        private readonly Particle particle;

        private readonly Vector3 teleportPosition;

        public Teleport(Particle particle, HeroId id, Vector3 position, float duration, bool start)
        {
            this.particle = particle;
            this.HeroId = id;
            this.teleportPosition = position;
            this.duration = duration;
            this.displayUntil = GameManager.RawGameTime + duration;
            this.mapTexture = this.HeroId.ToString();
            this.minimapTexture = this.HeroId.ToString();
            this.color = start ? Color.DarkOrange : Color.Red;
        }

        public HeroId HeroId { get; }

        public bool IsValid
        {
            get
            {
                return this.particle.IsValid && this.displayUntil >= GameManager.RawGameTime;
            }
        }

        public float RemainingDuration
        {
            get
            {
                return this.displayUntil - GameManager.RawGameTime;
            }
        }

        public void DrawOnMap(IMinimap minimap)
        {
            var position = minimap.WorldToScreen(this.teleportPosition, 45 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            RendererManager.DrawTexture("o9k.outline_red", position * 1.12f);
            RendererManager.DrawTexture(this.mapTexture, position, TextureType.RoundUnit);

            var abilityTexturePosition = position * 0.5f;
            abilityTexturePosition.X += abilityTexturePosition.Width * 0.8f;
            abilityTexturePosition.Y += abilityTexturePosition.Height * 0.6f;

            RendererManager.DrawTexture("o9k.outline_green_pct100", abilityTexturePosition * 1.2f);
            RendererManager.DrawTexture(AbilityTexture, abilityTexturePosition, TextureType.RoundAbility);

            position.Y += 50 * Hud.Info.ScreenRatio;

            RendererManager.DrawText(
                this.RemainingDuration.ToString("N1"),
                position,
                Color.White,
                FontFlags.Center,
                18 * Hud.Info.ScreenRatio);
        }

        public void DrawOnMinimap(IMinimap minimap)
        {
            const float MaxRadius = 22f;
            const float MinRadius = 15f;

            var remainingDuration = this.RemainingDuration;
            var position = minimap.WorldToMinimap(this.teleportPosition, 25 * Hud.Info.ScreenRatio);
            var radius = (((remainingDuration / this.duration) * (MaxRadius - MinRadius)) + MinRadius) * Hud.Info.ScreenRatio;
            var range = new Vector2((float)Math.Cos(-remainingDuration), (float)Math.Sin(-remainingDuration)) * radius;

            RendererManager.DrawCircle(position.Center, radius, this.color, 3);
            RendererManager.DrawLine(position.Center, position.Center + range, this.color, 2);
            RendererManager.DrawTexture(this.minimapTexture, position, TextureType.MiniUnit);
        }
    }
}