namespace O9K.Core.Helpers
{
    using System.Collections.Generic;

    using Divine;

    using SharpDX;

    public static class Drawer
    {
        private static readonly List<Particle> Particles = new List<Particle>();

        private static Particle greenParticle;

        private static Particle redParticle;

        public static void AddGreenCircle(Vector3 position)
        {
            var green = ParticleManager.CreateParticle(@"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf", ParticleAttachment.CustomOrigin, null);
            green.SetControlPoint(0, position);
            green.SetControlPoint(1, new Vector3(0, 255, 0));
            green.SetControlPoint(2, new Vector3(50, 255, 0));

            Particles.Add(green);
        }

        public static void AddRedCircle(Vector3 position)
        {
            var red = ParticleManager.CreateParticle(@"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf", ParticleAttachment.CustomOrigin, null);
            red.SetControlPoint(0, position);
            red.SetControlPoint(1, new Vector3(255, 0, 0));
            red.SetControlPoint(2, new Vector3(70, 255, 0));

            Particles.Add(red);
        }

        public static void Dispose()
        {
            foreach (var particleEffect in Particles)
            {
                particleEffect?.Dispose();
            }

            greenParticle?.Dispose();
            redParticle?.Dispose();
        }

        public static void DrawGreenCircle(Vector3 position)
        {
            if (greenParticle == null)
            {
                greenParticle = ParticleManager.CreateParticle(@"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf", ParticleAttachment.CustomOrigin, null);
                greenParticle.SetControlPoint(0, position);
                greenParticle.SetControlPoint(1, new Vector3(0, 255, 0));
                greenParticle.SetControlPoint(2, new Vector3(50, 255, 0));
            }

            greenParticle.SetControlPoint(0, position);
        }

        public static void DrawRedCircle(Vector3 position)
        {
            if (redParticle == null)
            {
                redParticle = ParticleManager.CreateParticle(@"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf", ParticleAttachment.CustomOrigin, null);
                greenParticle.SetControlPoint(0, position);
                redParticle.SetControlPoint(1, new Vector3(255, 0, 0));
                redParticle.SetControlPoint(2, new Vector3(70, 255, 0));
            }

            redParticle.SetControlPoint(0, position);
        }
    }
}