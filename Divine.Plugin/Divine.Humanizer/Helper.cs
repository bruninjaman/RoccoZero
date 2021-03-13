using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.Humanizer
{
    internal static class Helper
    {
        public static Vector3? Intersection(Vector3 lineStart, Vector3 lineEnd, Vector3 rayStart, Vector3 rayEnd)
        {
            var intersection = Intersection(lineStart.ToVector2(), lineEnd.ToVector2(), rayStart.ToVector2(), rayEnd.ToVector2());
            if (intersection == null)
            {
                return null;
            }

            return intersection.Value.ToVector3();
        }

        public static Vector2? Intersection(Vector2 lineStart, Vector2 lineEnd, Vector2 rayStart, Vector2 rayEnd)
        {
            var deltaACy = lineStart.Y - rayStart.Y;
            var deltaDCx = rayEnd.X - rayStart.X;
            var deltaACx = lineStart.X - rayStart.X;
            var deltaDCy = rayEnd.Y - rayStart.Y;
            var deltaBAx = lineEnd.X - lineStart.X;
            var deltaBAy = lineEnd.Y - lineStart.Y;

            var denominator = (deltaBAx * deltaDCy) - (deltaBAy * deltaDCx);
            var numerator = (deltaACy * deltaDCx) - (deltaACx * deltaDCy);

            var r = numerator / denominator;
            if (r < 0 || r > 1)
            {
                return null;
            }

            var s = ((deltaACy * deltaBAx) - (deltaACx * deltaBAy)) / denominator;
            if (s < 0 || s > 1)
            {
                return null;
            }

            return new Vector2(lineStart.X + (r * deltaBAx), lineStart.Y + (r * deltaBAy));
        }

        public static float Map(float start1, float start2, float end1, float end2, float value)
        {
            return end1 + (value - start1) * (end2 - end1) / (start2 - start1);
        }

        public static Vector3 GetLookAt(float distance)
        {
            return GetLookAt(CameraManager.Position, distance);
        }

        public static Vector3 GetLookAt(Vector3 position, float distance)
        {
            return new Vector3(position.X, position.Y + (distance / 2), position.Z);
        }

        public static Vector3 GetLookAtDefault()
        {
            return GetLookAtDefault(CameraManager.Position);
        }

        public static Vector3 GetLookAtDefault(Vector3 position)
        {
            return GetLookAt(position, CameraManager.DefaultDistance);
        }
    }
}