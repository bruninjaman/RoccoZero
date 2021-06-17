namespace O9K.Evader.Abilities.Base.Evadable.Components
{
    using Divine.Particle.Particles;

    internal interface IParticle
    {
        void AddParticle(Particle particle, string name);
    }
}