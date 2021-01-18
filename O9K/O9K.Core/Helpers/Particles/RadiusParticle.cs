namespace O9K.Core.Helpers.Particles
{
    using System;

    using Divine;

    using Entities.Units;

    using SharpDX;

    public class RadiusParticle : IDisposable
    {
        private const string ParticleName = "particles/ui_mouseactions/drag_selected_ring.vpcf";

        private readonly Particle particle;

        public RadiusParticle(Vector3 position, Vector3 color, float radius)
        {
            this.particle = ParticleManager.CreateParticle(ParticleName, position);
            this.SetData(color, radius);
        }

        public RadiusParticle(Unit9 unit, Vector3 color, float radius)
        {
            this.particle = ParticleManager.CreateParticle(ParticleName, unit.BaseUnit);
            this.SetData(color, radius);
        }

        public void ChangePosition(Vector3 position)
        {
            this.particle.SetControlPoint(0, position);
            this.particle.FullRestart();
        }

        public void Dispose()
        {
            this.particle.Dispose();
        }

        private void SetData(Vector3 color, float radius)
        {
            this.particle.SetControlPoint(1, color);
            this.particle.SetControlPoint(2, new Vector3(-radius, 255, 0));
        }
    }
}