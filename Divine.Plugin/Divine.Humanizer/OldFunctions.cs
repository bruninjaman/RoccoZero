using SharpDX;

namespace Divine.Humanizer
{
    internal static class OldFunctions
    {
        public static readonly Vector3 TopLeft = new Vector3(-2033.432f, 1320.753f, 0);

        public static readonly Vector3 TopRight = new Vector3(2032.374f, 1320.753f, 0);

        public static readonly Vector3 BottomLeft = new Vector3(-968.2466f, -628.3125f, 0);

        public static readonly Vector3 BottomRight = new Vector3(967.7427f, -628.3125f, 0);


        public static Vector3 GetTopLeftDefault(Vector3 lookAt)
        {
            return lookAt + TopLeft;
        }

        public static Vector3 GetTopRightDefault(Vector3 lookAt)
        {
            return lookAt + TopRight;
        }

        public static Vector3 GetBottomLeftDefault(Vector3 lookAt)
        {
            return lookAt + BottomLeft;
        }

        public static Vector3 GetBottomRightDefault(Vector3 lookAt)
        {
            return lookAt + BottomRight;
        }
    }
}