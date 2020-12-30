namespace O9K.Core.Helpers
{
    using Divine;

    public sealed class Sleeper
    {
        private bool sleeping;

        private float sleepTime;

        public Sleeper()
        {
        }

        public Sleeper(float seconds)
        {
            this.Sleep(seconds);
        }

        public bool IsSleeping
        {
            get
            {
                if (this.sleeping)
                {
                    this.sleeping = GameManager.RawGameTime < this.sleepTime;
                }

                return this.sleeping;
            }
        }

        public float RemainingSleepTime
        {
            get
            {
                if (!this.sleeping)
                {
                    return 0;
                }

                return this.sleepTime - GameManager.RawGameTime;
            }
        }

        public static implicit operator bool(Sleeper sleeper)
        {
            return sleeper.IsSleeping;
        }

        public void ExtendSleep(float seconds)
        {
            var gameTime = GameManager.RawGameTime;

            if (this.sleepTime > gameTime)
            {
                this.sleepTime += seconds;
            }
            else
            {
                this.sleepTime = gameTime + seconds;
            }

            this.sleeping = true;
        }

        public void Reset()
        {
            this.sleepTime = 0;
            this.sleeping = false;
        }

        public void Sleep(float seconds)
        {
            this.sleepTime = GameManager.RawGameTime + seconds;
            this.sleeping = true;
        }

        public void SleepUntil(float rawGameTime)
        {
            this.sleepTime = rawGameTime;
            this.sleeping = true;
        }
    }
}