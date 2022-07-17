using Divine.Extensions;

namespace O9K.Core.Extensions;

using System.Collections.Generic;
using System.Linq;
using Divine.Numerics;
using Entities.Units;

public static class UnitExtensions
{
    public static Vector3 GetCenterPosition(this IEnumerable<Unit9> units)
    {
        var array = units.ToArray();
        if (array.Length == 0)
        {
            return Vector3.Zero;
        }

        var position = array[0].Position;

        for (var i = 1; i < array.Length; i++)
        {
            position = (position + array[i].Position) / 2;
        }

        return position;
    }

    public static Vector3 Extend2D(this Unit9 unit, Vector3 to, float distance)
    {
        var v2 = unit.Position.ToVector2();
        var tov2 = to.ToVector2();

        return (v2 + (distance * (tov2 - v2).Normalized())).ToVector3();
    }

    public static Vector3 Extend2D(this Unit9 unit, Unit9 to, float distance)
    {
        var v2 = unit.Position.ToVector2();
        var tov2 = to.Position.ToVector2();

        return (v2 + (distance * (tov2 - v2).Normalized())).ToVector3();
    }

    public static Vector3 Extend2D(this Vector3 position, Unit9 to, float distance)
    {
        var v2 = position.ToVector2();
        var tov2 = to.Position.ToVector2();

        return (v2 + (distance * (tov2 - v2).Normalized())).ToVector3();
    }
}
