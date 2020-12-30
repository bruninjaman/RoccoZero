namespace O9K.Core.Extensions
{
    using System;

    using SharpDX;

    public static class Vector2Extensions
    {
        //public static float Distance(this Vector2 start, Vector2 end)
        //{
        //    return Vector2.Distance(start, end);
        //}

        //public static float DistanceSquared(this Vector2 start, Vector2 end)
        //{
        //    return Vector2.DistanceSquared(start, end);
        //}

        public static Vector2 IncreaseX(this Vector2 vector, float x)
        {
            vector.X += x;
            return vector;
        }

        public static Vector2 IncreaseY(this Vector2 vector, float y)
        {
            vector.Y += y;
            return vector;
        }

        public static Vector2 MultiplyX(this Vector2 vector, float x)
        {
            vector.X *= x;
            return vector;
        }

        public static Vector2 MultiplyY(this Vector2 vector, float y)
        {
            vector.Y *= y;
            return vector;
        }

        public static float Distance(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool onlyIfOnSegment = false, bool squared = false)
        {
            var objects = point.ProjectOn(segmentStart, segmentEnd);

            if (objects.IsOnSegment || onlyIfOnSegment == false)
            {
                return squared ? Vector2.DistanceSquared(objects.SegmentPoint, point) : Vector2.Distance(objects.SegmentPoint, point);
            }

            return float.MaxValue;
        }

        public static ProjectionInfo ProjectOn(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            var cx = point.X;
            var cy = point.Y;
            var ax = segmentStart.X;
            var ay = segmentStart.Y;
            var bx = segmentEnd.X;
            var by = segmentEnd.Y;
            var rL = (((cx - ax) * (bx - ax)) + ((cy - ay) * (@by - ay))) / ((float)Math.Pow(bx - ax, 2) + (float)Math.Pow(by - ay, 2));
            var pointLine = new Vector2(ax + (rL * (bx - ax)), ay + (rL * (@by - ay)));
            float rS;
            if (rL < 0)
            {
                rS = 0;
            }
            else if (rL > 1)
            {
                rS = 1;
            }
            else
            {
                rS = rL;
            }

            var isOnSegment = rS.CompareTo(rL) == 0;
            var pointSegment = isOnSegment ? pointLine : new Vector2(ax + (rS * (bx - ax)), ay + (rS * (@by - ay)));
            return new ProjectionInfo(isOnSegment, pointSegment, pointLine);
        }

        public struct ProjectionInfo
        {
            public bool IsOnSegment;

            public Vector2 LinePoint;

            public Vector2 SegmentPoint;

            public ProjectionInfo(bool isOnSegment, Vector2 segmentPoint, Vector2 linePoint)
            {
                this.IsOnSegment = isOnSegment;
                this.SegmentPoint = segmentPoint;
                this.LinePoint = linePoint;
            }
        }
    }
}