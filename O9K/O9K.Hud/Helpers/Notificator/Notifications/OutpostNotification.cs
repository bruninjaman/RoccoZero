namespace O9K.Hud.Helpers.Notificator.Notifications
{
    using Core.Helpers;

    using Divine;

    using SharpDX;

    internal sealed class OutpostNotification : Notification
    {
        private readonly Vector3[] positions;

        public OutpostNotification(bool playSound, Vector3[] positions)
            : base(playSound)
        {
            this.positions = positions;
            this.TimeToShow = 6;
        }

        public override void Draw(RectangleF position, IMinimap minimap)
        {
            var notificationSize = GetNotificationSize(position);
            var textureSize = GetTextureSize(notificationSize);
            var opacity = this.GetOpacity();

            RendererManager.DrawTexture("o9k.notification_bg", notificationSize, opacity);
            RendererManager.DrawTexture("o9k.outpost", textureSize, opacity);

            foreach (var vector3 in this.positions)
            {
                RendererManager.DrawTexture("o9k.ping", minimap.WorldToMinimap(vector3, 25 * Hud.Info.ScreenRatio * this.GetPingSize()));
            }
        }

        private static RectangleF GetNotificationSize(RectangleF position)
        {
            var rec = position;

            rec.X = position.Center.X;
            rec.Width = position.Width * 0.5f;

            return rec;
        }

        private static RectangleF GetTextureSize(RectangleF position)
        {
            var rec = new RectangleF();

            rec.Width = position.Width * 0.5f;
            rec.Height = position.Height;
            rec.X = position.Center.X - (rec.Width / 2f);
            rec.Y = position.Y;

            return rec;
        }
    }
}