namespace O9K.Hud.Helpers.Notificator.Notifications
{
    using Divine;

    using SharpDX;

    internal sealed class AbilityNotification : Notification
    {
        private readonly string abilityName;

        private readonly string heroName;

        public AbilityNotification(string heroName, string abilityName)
        {
            this.heroName = heroName;
            this.abilityName = abilityName;
            this.TimeToShow = 3;
        }

        public override void Draw(RectangleF position, IMinimap minimap)
        {
            var heroPosition = GetHeroPosition(position);
            var abilityPosition = GetAbilityPosition(position, heroPosition);
            var arrowPosition = GetArrowPosition(position, heroPosition, abilityPosition);
            var opacity = this.GetOpacity();

            if (this.heroName == null)
            {
                position.X += heroPosition.Width;
                RendererManager.DrawTexture("o9k.notification_bg", position, opacity);
            }
            else
            {
                RendererManager.DrawTexture("o9k.notification_bg", position, opacity);
                RendererManager.DrawTexture(this.heroName, heroPosition, TextureType.Unit, opacity);
            }

            RendererManager.DrawTexture("o9k.ping", arrowPosition, opacity);
            RendererManager.DrawTexture(this.abilityName, abilityPosition, TextureType.Ability, opacity);
        }

        private static RectangleF GetAbilityPosition(RectangleF position, RectangleF heroPosition)
        {
            var rec = heroPosition;
            rec.Width *= 0.75f;
            rec.X = position.Right - (position.Width * 0.05f) - rec.Width;

            return rec;
        }

        private static RectangleF GetArrowPosition(RectangleF position, RectangleF heroPosition, RectangleF itemPosition)
        {
            var rec = new RectangleF();

            rec.Width = position.Width * 0.18f;
            rec.Height = position.Height * 0.6f;
            rec.X = ((heroPosition.Right + itemPosition.Left) / 2f) - (rec.Width / 2f);
            rec.Y = (position.Y + (position.Height / 2f)) - (rec.Height / 2);

            return rec;
        }

        private static RectangleF GetHeroPosition(RectangleF position)
        {
            var rec = position;

            rec.X += position.Width * 0.05f;
            rec.Y += position.Height * 0.15f;
            rec.Width = position.Width * 0.3f;
            rec.Height = position.Height * 0.7f;

            return rec;
        }
    }
}