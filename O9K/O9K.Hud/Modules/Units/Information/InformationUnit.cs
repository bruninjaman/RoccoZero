namespace O9K.Hud.Modules.Units.Information
{
    using System;

    using Core.Entities.Heroes;
    using Core.Entities.Units;

    using Divine;

    using SharpDX;

    internal class InformationUnit
    {
        public InformationUnit(Unit9 unit)
        {
            this.Unit = unit;
        }

        public int Hits { get; private set; }

        public bool ShouldDraw
        {
            get
            {
                return this.Unit.IsValid && this.Unit.IsVisible && this.Unit.IsAlive;
            }
        }

        public Unit9 Unit { get; }

        public void DrawInformation(float mySpeed, bool showDamage, bool showSpeed, Vector2 position, float size)
        {
            var hpBar = this.Unit.HealthBarPosition;
            if (hpBar.IsZero)
            {
                return;
            }

            var hpSize = this.Unit.HealthBarSize;
            var iconSize = size * 0.75f;

            var iconPosition = hpBar + position + new Vector2(hpSize.X, size - iconSize);
            var textPosition = hpBar + position + new Vector2(hpSize.X + iconSize + 3, 0);

            if (showDamage)
            {
                RendererManager.DrawTexture("o9k.attack_minimalistic", new RectangleF(iconPosition.X, iconPosition.Y, iconSize, iconSize));
                RendererManager.DrawText(this.Hits == 0 ? "?" : this.Hits.ToString(), textPosition, Color.White, size);

                iconPosition += new Vector2(0, size);
                textPosition += new Vector2(0, size);
            }

            if (showSpeed)
            {
                RendererManager.DrawTexture("o9k.speed_minimalistic", new RectangleF(iconPosition.X, iconPosition.Y, iconSize, iconSize));

                var speed = (int)(mySpeed - this.Unit.Speed);
                if (speed >= 0)
                {
                    RendererManager.DrawText(speed.ToString(), textPosition, Color.White, size);
                }
                else
                {
                    RendererManager.DrawText((speed * -1).ToString(), textPosition, Color.DarkOrange, size);
                }
            }
        }

        public void UpdateDamage(Hero9 myHero)
        {
            this.Hits = 0;

            if (!this.Unit.IsAlive)
            {
                return;
            }

            var count = (int)Math.Ceiling(
                this.Unit.Health / (myHero.GetAttackDamage(this.Unit) - (this.Unit.HealthRegeneration * myHero.SecondsPerAttack)));

            if (count > 0)
            {
                this.Hits = count;
            }
        }
    }
}