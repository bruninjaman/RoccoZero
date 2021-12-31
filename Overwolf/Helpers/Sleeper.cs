using Divine.Game;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overwolf.Helpers
{
    public sealed class Sleeper
    {
        private bool isSleeping;

        private float sleepTime;

        public bool Sleeping
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

        public void Reset()
        {
            sleepTime = 0;
            isSleeping = false;
        }

        public void Sleep(float milliseconds)
        {
            sleepTime = GameManager.Time * 1000 + milliseconds;
            isSleeping = true;
        }
    }
}
