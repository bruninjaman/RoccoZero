using SharpDX;

namespace Divine.SkywrathMage.TargetSelector.TargetEffects.EffectType
{
    internal class BaseEffectType
    {
        protected Unit Owner { get; } = EntityManager.LocalHero;

        public void Remove()
        {
            ParticleManager.RemoveParticle("Divine.TargetEffects");
        }

        public int Red { get; set; }

        public int Green { get; set; }

        public int Blue { get; set; }

        protected Color Color
        {
            get
            {
                return new Color(Red, Green, Blue);
            }
        }
    }
}