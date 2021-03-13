using Divine.SDK.Extensions;

using SharpDX;

namespace Divine.Humanizer
{
    internal sealed class Camera
    {
        private readonly Humanizer Humanizer;

        public Camera(Humanizer humanizer)
        {
            Humanizer = humanizer;

            Position = CameraManager.Position;

            UpdateManager.CreateIngameUpdate(OnUpdate);
        }

        private void OnUpdate()
        {
            LookAt = Helper.GetLookAtDefault(Position);

            TopLeft = OldFunctions.GetTopLeftDefault(LookAt);
            TopRight = OldFunctions.GetTopRightDefault(LookAt);
            BottomLeft = OldFunctions.GetBottomLeftDefault(LookAt);
            BottomRight = OldFunctions.GetBottomRightDefault(LookAt);

            TopLeftTrigger = LookAt + (OldFunctions.TopLeft * 0.50f);
            TopRightTrigger = LookAt + (OldFunctions.TopRight * 0.50f);
            BottomLeftTrigger = LookAt + (OldFunctions.BottomLeft * 0.50f);
            BottomRightTrigger = LookAt + (OldFunctions.BottomRight * 0.50f);

            var mousePosition = MousePosition.SetZ(0);
            if (mousePosition.X == float.MaxValue || mousePosition.Y == float.MaxValue)
            {
                return;
            }

            Intersection = Helper.Intersection(TopLeftTrigger, TopRightTrigger, LookAt, mousePosition)
                        ?? Helper.Intersection(TopRightTrigger, BottomRightTrigger, LookAt, mousePosition)
                        ?? Helper.Intersection(BottomRightTrigger, BottomLeftTrigger, LookAt, mousePosition)
                        ?? Helper.Intersection(BottomLeftTrigger, TopLeftTrigger, LookAt, mousePosition);

            if (Intersection != null && TEST)
            {
                var timer = (GameManager.RawGameTime - Time) * 5;
                if (timer > 1)
                {
                    test = false;
                    return;
                }

                Move = LookAt.Extend(mousePosition, Intersection.Value.Distance(mousePosition) * timer);

                Position = new Vector3(Move.X, Move.Y - (CameraManager.DefaultDistance / 2f), 0);
            }

            if (Humanizer.Menu.DrawSwitcher)
            {
                ParticleManager.LineParticle("MumanizerCamera.TopLeft", TopLeft, TopRight, 20, Color.Aqua);
                ParticleManager.LineParticle("MumanizerCamera.TopRight", TopRight, BottomRight, 20, Color.Aqua);
                ParticleManager.LineParticle("MumanizerCamera.BottomRight", BottomRight, BottomLeft, 20, Color.Aqua);
                ParticleManager.LineParticle("MumanizerCamera.BottomLeft", BottomLeft, TopLeft, 20, Color.Aqua);
            }
            else
            {
                ParticleManager.RemoveParticle("MumanizerCamera.TopLeft");
                ParticleManager.RemoveParticle("MumanizerCamera.TopRight");
                ParticleManager.RemoveParticle("MumanizerCamera.BottomRight");
                ParticleManager.RemoveParticle("MumanizerCamera.BottomLeft");
            }
        }

        private float Time;

        private bool test;

        public bool TEST
        {
            get
            {
                return test;
            }

            set
            {
                if (test)
                {
                    return;
                }

                Time = GameManager.RawGameTime;
                test = value;
            }
        }

        public Vector3 Position { get; private set; }

        public Vector3 LookAt { get; private set; }

        public Vector3 TopLeft { get; private set; }

        public Vector3 TopRight { get; private set; }

        public Vector3 BottomLeft { get; private set; }

        public Vector3 BottomRight { get; private set; }

        public Vector3 TopLeftTrigger { get; private set; }

        public Vector3 TopRightTrigger { get; private set; }

        public Vector3 BottomLeftTrigger { get; private set; }

        public Vector3 BottomRightTrigger { get; private set; }

        public Vector3? Intersection { get; private set; }

        public Vector3 Move { get; private set; }

        public Vector3 MousePosition { get; set; }
    }
}