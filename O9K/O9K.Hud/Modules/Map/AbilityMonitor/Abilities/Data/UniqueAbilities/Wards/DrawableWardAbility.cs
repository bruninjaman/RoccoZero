namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.Wards
{
    using System;

    using Base;

    using Core.Helpers;

    using Divine;

    using Helpers;

    using SharpDX;

    internal class DrawableWardAbility : DrawableAbility
    {
        public DrawableWardAbility()
        {
            this.AddedTime = GameManager.RawGameTime;
        }

        public string AbilityUnitName { get; set; }

        public float AddedTime { get; set; }

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
            this.Position = unit.Position;

            this.DrawRange();
        }

        public override void DrawOnMap(IMinimap minimap)
        {
            var position = minimap.WorldToScreen(this.Position, 30 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            var pct = (int)((GameManager.RawGameTime - this.AddedTime) / this.Duration * 100);
            RendererManager.DrawTexture("o9k.outline_red", position * 1.2f);
            RendererManager.DrawTexture("o9k.outline_black" + pct, position * 1.25f);
            RendererManager.DrawTexture(this.AbilityTexture, position, TextureType.RoundAbility);

            position.Y += 30 * Hud.Info.ScreenRatio;
            position *= 2;

            RendererManager.DrawText(
                TimeSpan.FromSeconds(this.Duration - (GameManager.RawGameTime - this.AddedTime)).ToString(@"m\:ss"),
                position,
                Color.White,
                FontFlags.Center | FontFlags.VerticalCenter,
                18 * Hud.Info.ScreenRatio);
        }

        public override void DrawOnMinimap(IMinimap minimap)
        {
            if (this.Unit?.IsVisible == true)
            {
                return;
            }

            var position = minimap.WorldToMinimap(this.Position, 15 * Hud.Info.ScreenRatio);
            if (position.IsZero)
            {
                return;
            }

            RendererManager.DrawTexture("o9k.minimap_" + this.AbilityTexture, position);
        }
    }
}