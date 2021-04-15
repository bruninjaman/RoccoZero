namespace O9K.Hud.Helpers.Notificator.Notifications
{
    using System;
    using System.Threading.Tasks;

    using Divine;

    using SharpDX;

    internal abstract class Notification
    {
        private readonly bool playSound;

        private float startDisplayTime;

        private float stopDisplayTime;

        protected Notification(bool playSound = false)
        {
            this.playSound = playSound;
        }

        public bool IsExpired
        {
            get
            {
                return GameManager.RawGameTime > this.stopDisplayTime;
            }
        }

        protected int PingCycleCount { get; set; } = 4;

        protected int TimeToShow { get; set; } = 4;

        public abstract void Draw(RectangleF position, IMinimap minimap);

        public virtual bool OnClick()
        {
            return false;
        }

        public virtual void Pushed()
        {
            this.startDisplayTime = GameManager.RawGameTime;
            this.stopDisplayTime = this.startDisplayTime + this.TimeToShow;

            if (this.playSound)
            {
                this.PlaySound();
            }
        }

        protected float GetOpacity()
        {
            var opacity = 1f;
            var time = GameManager.RawGameTime;

            if (this.startDisplayTime + 0.5f > time)
            {
                opacity = (time - this.startDisplayTime) * 2;
            }
            else if (time + 0.5f > this.stopDisplayTime)
            {
                opacity = (this.stopDisplayTime - time) * 2;
            }

            return Math.Max(opacity, 0);
        }

        protected float GetPingSize()
        {
            var size = 0f;
            var time = GameManager.RawGameTime;
            var cycle = (this.TimeToShow / this.PingCycleCount);
            var halfCycle = cycle / 2f;

            for (var i = 0; i < this.PingCycleCount; i++)
            {
                var increaseStart = this.startDisplayTime + (cycle * i);
                var decreaseStart = increaseStart + halfCycle;

                if (time >= increaseStart && time < decreaseStart)
                {
                    size = (time - increaseStart) / halfCycle;
                    break;
                }

                if (time >= decreaseStart && time < decreaseStart + halfCycle)
                {
                    size = 1 - ((time - decreaseStart) / halfCycle);
                    break;
                }
            }

            return size;
        }

        protected void PlaySound()
        {
            UpdateManager.BeginInvoke(
                async () =>
                {
                    var hero = EntityManager.LocalHero;

                    //hero.PlaySound("General.Ping"); TODO
                    await Task.Delay(450);
                    //hero.PlaySound("General.Ping");
                });
        }
    }
}