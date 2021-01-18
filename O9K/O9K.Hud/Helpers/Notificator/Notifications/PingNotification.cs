namespace O9K.Hud.Helpers.Notificator.Notifications
{
    using Core.Helpers;

    using Divine;

    using SharpDX;

    internal class PingNotification : Notification
    {
        private readonly Vector3 pingPosition;

        public PingNotification(Vector3 position, bool playSound)
            : base(playSound)
        {
            this.pingPosition = position;
            this.TimeToShow = 5;
        }

        public override void Draw(RectangleF position, IMinimap minimap)
        {
            RendererManager.DrawTexture("o9k.ping", minimap.WorldToMinimap(this.pingPosition, 25 * Hud.Info.ScreenRatio * this.GetPingSize()));
        }
    }
}