using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowEnemyRoles
{
    internal enum LaneSelectonFlags
    {
        Unknown = 0,
        SafeLane = 1 << 0,
        OffLane = 1 << 1,
        MidLane = 1 << 2,
        SoftSupport = 1 << 3,
        HardSupport = 1 << 4,
    }
}
