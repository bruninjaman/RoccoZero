namespace O9K.Core.Managers.Particle
{
    using Divine;

    using SharpDX;

    public sealed class Particle9
    {
        internal Particle9(Particle effect, string effectName, Entity sender, bool isReleased)
        {
            this.Effect = effect;
            this.Name = effectName;
            //this.Released = isReleased;
            // this.CreateTime = Game.RawGameTime;

            if (sender != null && !(sender is Player))
            {
                this.Sender = sender;
                this.SenderHandle = sender.Handle;
            }
        }

        //   public float CreateTime { get; }

        public Particle Effect { get; }

        public bool IsValid
        {
            get
            {
                return this.Effect.IsValid;
            }
        }

        public string Name { get; }

        public Vector3 Position
        {
            get
            {
                return this.Effect.Position;
            }
        }

        //public bool Released { get; }

        public Entity Sender { get; }

        public uint? SenderHandle { get; }

        public Vector3 GetControlPoint(int id)
        {
            return this.Effect.GetControlPoint(id);
        }
    }
}