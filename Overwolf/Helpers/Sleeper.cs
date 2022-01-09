using Divine.Game;

namespace Overwolf.Helpers
{
    internal sealed class Sleeper
    {
        private bool isSleeping;

        private float sleepTime;

        internal bool Sleeping
        {
            get
            {
                if (!isSleeping)
                {
                    return false;
                }

                isSleeping = GameManager.Time * 1000 < sleepTime;
                return isSleeping;
            }
        }

        internal void Reset()
        {
            sleepTime = 0;
            isSleeping = false;
        }

        internal void Sleep(float milliseconds)
        {
            sleepTime = GameManager.Time * 1000 + milliseconds;
            isSleeping = true;
        }
    }
}